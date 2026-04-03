using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement.Domain.DI
{
    public static class DomainDependencies
    {
        public static IServiceCollection AddDomain(this IServiceCollection servises)
        {
            return servises;
        }
    }
}
