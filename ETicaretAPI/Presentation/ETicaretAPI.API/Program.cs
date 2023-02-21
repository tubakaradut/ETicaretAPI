using ETicaretAPI.API.Configurations.ColumnWriter;
using ETicaretAPI.API.Extensions;
using ETicaretAPI.API.Filters;
using ETicaretAPI.Application;
using ETicaretAPI.Infrastructure.Registrations;
using ETicaretAPI.Persistence.Registrations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.IdentityModel.Tokens;
using NpgsqlTypes;
using Serilog;
using Serilog.Context;
using Serilog.Core;
using Serilog.Sinks.PostgreSQL;
using System.Security.Claims;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //clienttan gelen request neticisinde oluþturulan httpcontext nesnesini katmanlardaki classlar üzerinden eriþebilmemizi saðlamak için yani  User.Identity.Name ulaþmak için
        builder.Services.AddHttpContextAccessor();

        //yazdýðýmýz extension methodu tanýmlarýz.
        builder.Services.AddPersistenceService();
        builder.Services.AddApplicationService();
        builder.Services.AddInfrastructureService();

        //default olan loglama iþlemini serilog olarak yapmak için 
        Logger log = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/log.txt")
            .WriteTo.PostgreSQL(builder.Configuration.GetConnectionString("PostgreSQL"), "logs",//veritabanýnda logs adýnda tablo oluþturup orada loglamalar tutulacak
                needAutoCreateTable: true,
                 columnOptions: new Dictionary<string, ColumnWriterBase> //loglama mekanizmasýnda hangi kolonlarýn tutulacaksa onlarý belirtiyoruz
                {
            {"message", new RenderedMessageColumnWriter(NpgsqlDbType.Text)},
            {"message_template", new MessageTemplateColumnWriter(NpgsqlDbType.Text)},
            {"level", new LevelColumnWriter(true , NpgsqlDbType.Varchar)},
            {"time_stamp", new TimestampColumnWriter(NpgsqlDbType.Timestamp)},
            {"exception", new ExceptionColumnWriter(NpgsqlDbType.Text)},
            {"log_event", new LogEventSerializedColumnWriter(NpgsqlDbType.Json)},
            {"user_name", new UserNameColumnWriter()}
                })
            //.WriteTo.Seq(builder.Configuration["Seq:ServerURL"]) //seq arayüzü ile clientta loglama iþlemlerini gösterebilmek için
            .Enrich.FromLogContext() // logun contextine harici eklemiþ olduðumuz propertleri kullanabilmek için 
            .MinimumLevel.Information()
            .CreateLogger();

        builder.Host.UseSerilog(log);

        //httploggig yapýsýný ekliyoruz ve kullanýyoruz ki kullanýcýya dair tüm bilgilerin gelmesi için
        builder.Services.AddHttpLogging(logging =>
        {
            logging.LoggingFields = HttpLoggingFields.All;
            logging.RequestHeaders.Add("sec-ch-ua"); //kullanýcýya ait tüm bilgiler
            logging.ResponseHeaders.Add("Mini-E-Ticaret");
            logging.MediaTypeOptions.AddText("application/javascript");
            logging.RequestBodyLogLimit = 4096;
            logging.ResponseBodyLogLimit = 4096;
        });

       

        //Uygulamamýz için CORS politikalarýný dü<enlemek için; doðal Same-Origin Policy yöntemlerini hafifletme politikasý için yazýlýr.
        builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
            policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials()
        ));


        //JWT token ile iþlemler token doðrulama iþlemleri
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer("Admin", options =>
            {
                options.TokenValidationParameters = new()
                {
                    //Doðrulacaðýmýz deðerler belirlenir...
                    ValidateAudience = true, //Olusturulacak token deðerini kimlerin/hangi originlerin/sitelerin kullanýcý belirlediðimiz deðerdir.
                    ValidateIssuer = true, //Olusturulacak token deðerini kimin daðýttýðýný ifade edeceðimiz alandýr. 
                    ValidateLifetime = true, //Oluþturulan token deðerinin süresini kontrol edecek olan doðrulamadýr.
                    ValidateIssuerSigningKey = true, //Üretilecek token deðerinin uygulamamýza ait bir deðer olduðunu ifade eden suciry key verisinin doðrulanmasýdýr.


                    //Hangi deðerlerle doðrulanacaðý belirlenir...
                    ValidAudience = builder.Configuration["Token:Audience"],
                    ValidIssuer = builder.Configuration["Token:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"])),

                    //expires jwt token süresini ayarlýyor
                    LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires != null ? expires > DateTime.UtcNow : false,

                    NameClaimType = ClaimTypes.Name //JWT üzerinde Name claimne karþýlýk gelen deðeri User.Identity.Name propertysinden elde edebiliriz. yani hangi kullanýcýn istek yaptýðýný öðrenebilmek (loglamada da)için yazýlýr.
                };
            });

        builder.Services.AddAuthorization();


        builder.Services.AddControllers(options => options.Filters.Add<CustomAuthorizeFilterRole>());


        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //extension olarak yazdýðýmýz exceptionhandleri çaðýrýyoruz.
        app.ConfigureExceptionHandler<Program>(app.Services.GetRequiredService<ILogger<Program>>());

        //serilog yapmak istediðimiz yapýlarýn en  üzerine konulmalý...
        app.UseSerilogRequestLogging();

        //uygulamada yapýlan requestleride log mekanizmasý ile görebilmek için eklenir.
        app.UseHttpLogging();

        //yazmýþ olduðumuz cors politikasýný çalýþtýr,kullan
        app.UseCors();

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();


        //kullanýcýdan gelen her istekte username yakalayabilmek için middleware oluþturup gelen istekte authentication olmus bir kullanýcý varsa ve bu kullanýcý bilgisinin name deðeri varsa bunu yakalayýp username propertysine karþýlýk olarak logun contextine atýlacak.
        app.Use(async (context, next) => //next bir sonraki delegeteyi temsil ediyor yani bu middleware çalýþtýrdýktan sonra ilerle diðerlerini çalýþtýr demek için
        {

            // user varsa identitye git - identity null deðilse authenticete true ise name getir deðilse null getir demek
            var username = context.User?.Identity?.IsAuthenticated != null || true ? context.User.Identity.Name : null;

            LogContext.PushProperty("user_name", username);

            await next();

        });


        app.MapControllers();

        app.Run();
    }
}