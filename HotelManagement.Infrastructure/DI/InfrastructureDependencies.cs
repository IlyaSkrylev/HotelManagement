using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement.Infrastructure.DI
{
    public static class InfrastructureDependencies
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection servises)
        {
            return servises;
        }
    }
}
