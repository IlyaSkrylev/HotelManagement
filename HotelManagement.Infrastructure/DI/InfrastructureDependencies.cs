using HotelManagement.Application.Abstractions;
using HotelManagement.Infrastructure.Data;
using HotelManagement.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagement.Infrastructure.DI
{
    public static class InfrastructureDependencies
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString, npgsqlOptions =>  
                {
                    npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                }));

            services.AddScoped<IApplicationDbContext>(provider =>
                provider.GetRequiredService<ApplicationDbContext>());

            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            services.AddHttpContextAccessor();
            return services;
        }
    }
}
