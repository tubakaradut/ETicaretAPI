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

        //clienttan gelen request neticisinde olu�turulan httpcontext nesnesini katmanlardaki classlar �zerinden eri�ebilmemizi sa�lamak i�in yani  User.Identity.Name ula�mak i�in
        builder.Services.AddHttpContextAccessor();

        //yazd���m�z extension methodu tan�mlar�z.
        builder.Services.AddPersistenceService();
        builder.Services.AddApplicationService();
        builder.Services.AddInfrastructureService();

        //default olan loglama i�lemini serilog olarak yapmak i�in 
        Logger log = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/log.txt")
            .WriteTo.PostgreSQL(builder.Configuration.GetConnectionString("PostgreSQL"), "logs",//veritaban�nda logs ad�nda tablo olu�turup orada loglamalar tutulacak
                needAutoCreateTable: true,
                 columnOptions: new Dictionary<string, ColumnWriterBase> //loglama mekanizmas�nda hangi kolonlar�n tutulacaksa onlar� belirtiyoruz
                {
            {"message", new RenderedMessageColumnWriter(NpgsqlDbType.Text)},
            {"message_template", new MessageTemplateColumnWriter(NpgsqlDbType.Text)},
            {"level", new LevelColumnWriter(true , NpgsqlDbType.Varchar)},
            {"time_stamp", new TimestampColumnWriter(NpgsqlDbType.Timestamp)},
            {"exception", new ExceptionColumnWriter(NpgsqlDbType.Text)},
            {"log_event", new LogEventSerializedColumnWriter(NpgsqlDbType.Json)},
            {"user_name", new UserNameColumnWriter()}
                })
            //.WriteTo.Seq(builder.Configuration["Seq:ServerURL"]) //seq aray�z� ile clientta loglama i�lemlerini g�sterebilmek i�in
            .Enrich.FromLogContext() // logun contextine harici eklemi� oldu�umuz propertleri kullanabilmek i�in 
            .MinimumLevel.Information()
            .CreateLogger();

        builder.Host.UseSerilog(log);

        //httploggig yap�s�n� ekliyoruz ve kullan�yoruz ki kullan�c�ya dair t�m bilgilerin gelmesi i�in
        builder.Services.AddHttpLogging(logging =>
        {
            logging.LoggingFields = HttpLoggingFields.All;
            logging.RequestHeaders.Add("sec-ch-ua"); //kullan�c�ya ait t�m bilgiler
            logging.ResponseHeaders.Add("Mini-E-Ticaret");
            logging.MediaTypeOptions.AddText("application/javascript");
            logging.RequestBodyLogLimit = 4096;
            logging.ResponseBodyLogLimit = 4096;
        });

       

        //Uygulamam�z i�in CORS politikalar�n� d�<enlemek i�in; do�al Same-Origin Policy y�ntemlerini hafifletme politikas� i�in yaz�l�r.
        builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
            policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials()
        ));


        //JWT token ile i�lemler token do�rulama i�lemleri
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer("Admin", options =>
            {
                options.TokenValidationParameters = new()
                {
                    //Do�rulaca��m�z de�erler belirlenir...
                    ValidateAudience = true, //Olusturulacak token de�erini kimlerin/hangi originlerin/sitelerin kullan�c� belirledi�imiz de�erdir.
                    ValidateIssuer = true, //Olusturulacak token de�erini kimin da��tt���n� ifade edece�imiz aland�r. 
                    ValidateLifetime = true, //Olu�turulan token de�erinin s�resini kontrol edecek olan do�rulamad�r.
                    ValidateIssuerSigningKey = true, //�retilecek token de�erinin uygulamam�za ait bir de�er oldu�unu ifade eden suciry key verisinin do�rulanmas�d�r.


                    //Hangi de�erlerle do�rulanaca�� belirlenir...
                    ValidAudience = builder.Configuration["Token:Audience"],
                    ValidIssuer = builder.Configuration["Token:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"])),

                    //expires jwt token s�resini ayarl�yor
                    LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires != null ? expires > DateTime.UtcNow : false,

                    NameClaimType = ClaimTypes.Name //JWT �zerinde Name claimne kar��l�k gelen de�eri User.Identity.Name propertysinden elde edebiliriz. yani hangi kullan�c�n istek yapt���n� ��renebilmek (loglamada da)i�in yaz�l�r.
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

        //extension olarak yazd���m�z exceptionhandleri �a��r�yoruz.
        app.ConfigureExceptionHandler<Program>(app.Services.GetRequiredService<ILogger<Program>>());

        //serilog yapmak istedi�imiz yap�lar�n en  �zerine konulmal�...
        app.UseSerilogRequestLogging();

        //uygulamada yap�lan requestleride log mekanizmas� ile g�rebilmek i�in eklenir.
        app.UseHttpLogging();

        //yazm�� oldu�umuz cors politikas�n� �al��t�r,kullan
        app.UseCors();

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();


        //kullan�c�dan gelen her istekte username yakalayabilmek i�in middleware olu�turup gelen istekte authentication olmus bir kullan�c� varsa ve bu kullan�c� bilgisinin name de�eri varsa bunu yakalay�p username propertysine kar��l�k olarak logun contextine at�lacak.
        app.Use(async (context, next) => //next bir sonraki delegeteyi temsil ediyor yani bu middleware �al��t�rd�ktan sonra ilerle di�erlerini �al��t�r demek i�in
        {

            // user varsa identitye git - identity null de�ilse authenticete true ise name getir de�ilse null getir demek
            var username = context.User?.Identity?.IsAuthenticated != null || true ? context.User.Identity.Name : null;

            LogContext.PushProperty("user_name", username);

            await next();

        });


        app.MapControllers();

        app.Run();
    }
}