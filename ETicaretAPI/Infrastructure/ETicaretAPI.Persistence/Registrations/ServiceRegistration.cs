
using ETicaretAPI.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETicaretAPI.Application.Repositories.CustomerRepo;
using ETicaretAPI.Persistence.Repositories.CustomerRepo;
using ETicaretAPI.Application.Repositories.OrderRepo;
using ETicaretAPI.Persistence.Repositories.OrderRepo;
using ETicaretAPI.Persistence.Repositories.ProductRepo;
using ETicaretAPI.Application.Repositories.ProductRepo;
using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Persistence.Repositories;
using ETicaretAPI.Domain.Entities.Identiy;
using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Persistence.Services;
using ETicaretAPI.Application.Abstractions.Services.Authentications;
using ETicaretAPI.Persistence.Repositories.MenuuRepo;
using ETicaretAPI.Application.Repositories.MenuuRepo;
using ETicaretAPI.Application.Repositories.EndpointRepo;
using ETicaretAPI.Persistence.Repositories.EndpointRepo;
using ETicaretAPI.Application.Repositories.BasketItemRepo;
using ETicaretAPI.Persistence.Repositories.BasketItemRepo;
using ETicaretAPI.Application.Repositories.BasketRepo;
using ETicaretAPI.Persistence.Repositories.BasketRepo;
using ETicaretAPI.Application.Repositories.CompletedOrderRepo;
using ETicaretAPI.Persistence.Repositories.CompletedOrderRepo;

namespace ETicaretAPI.Persistence.Registrations
{
    //extension method yazarak IOC containera eklenmesi sağlanır
    public static class ServiceRegistration
    {
        public static void AddPersistenceService(this IServiceCollection services)
        {
            
            services.AddDbContext<ETicaretAPIDbContext>(options => options.UseNpgsql(Configuration.ConnectionString));

            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.Password.RequiredLength = 3;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
            }).AddEntityFrameworkStores<ETicaretAPIDbContext>();

            services.AddScoped<ICustomerReadRepository, CustomerReadRepository>();
            services.AddScoped<ICustomerWriteRepository, CustomerWriteRepository>();
                     
            services.AddScoped<IOrderReadRepository, OrderReadRepository>();
            services.AddScoped<IOrderWriteRepository, OrderWriteRepository>();
                   
            services.AddScoped<IProductReadRepository, ProductReadRepository>();
            services.AddScoped<IProductWriteRepository, ProductWriteRepository>();

            services.AddScoped<IBasketItemReadRepository,BasketItemReadRepository>();  
            services.AddScoped<IBasketItemWriteRepository,BasketItemWriteRepository>();  
            services.AddScoped<IBasketReadRepository,BasketReadRepository>();  
            services.AddScoped<IBasketWriteRepository,BasketWriteRepository>();

            services.AddScoped<IMenuuWriteRepository, MenuuWriteRepository>();
            services.AddScoped<IMenuuReadRepository, MenuuReadRepository>();
            services.AddScoped<IEndpointReadRepository, EndpointReadRepository>();
            services.AddScoped<IEndpointWriteRepository, EndpointWriteRepository>();

            services.AddScoped<ICompletedOrderReadRepository, CompletedOrderReadRepository>();
            services.AddScoped<ICompletedOrderWriteRepository, CompletedOrderWriteRepository>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IBasketService, BasketService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IProductService,ProductService>();


            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IExternalAuthentication, AuthService>();
            services.AddScoped<IInternalAuthentication, AuthService>();

            services.AddScoped<IRoleService, RoleService>();

            services.AddScoped<IAuthorizationEndpointService,AuthorizationEndpointService>();

    
        }
    }
}
