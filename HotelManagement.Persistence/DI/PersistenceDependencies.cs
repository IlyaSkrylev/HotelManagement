using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement.Persistence.DI
{
    public static class PersistenceDependencies
    {
        public static IServiceCollection AddPersistence(this IServiceCollection servises)
        {
            return servises;
        }
    }
}
