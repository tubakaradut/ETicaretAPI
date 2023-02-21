using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ETicaretAPI.Application
{
    //extension method yazarak IOC containera eklenmesi sağlanır
    public static class ServiceRegistration
    {
        public static void AddApplicationService(this IServiceCollection services)
        {

            services.AddMediatR(typeof(ServiceRegistration));

            
        }
    }
}
