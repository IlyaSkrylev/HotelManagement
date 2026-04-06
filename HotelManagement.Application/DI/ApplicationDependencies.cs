using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HotelManagement.Application.DI
{
    public static class ApplicationDependencies
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            return services;
        }

    }
}
