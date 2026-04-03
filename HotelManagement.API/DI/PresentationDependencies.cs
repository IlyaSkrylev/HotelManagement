using HotelManagement.API.Common;
using System.Reflection;

namespace HotelManagement.API.DI
{
    public static class PresentationDependencies
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services)
        {
            services.AddOpenApi();

            var endpointTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && !t.IsInterface && typeof(IEndpoint).IsAssignableFrom(t));

            foreach (var type in endpointTypes)
            {
                services.AddScoped(typeof(IEndpoint), type);
            }

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            return services;
        }
    }
}
