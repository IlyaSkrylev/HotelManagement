using HotelManagement.Application.DI;
using HotelManagement.Infrastructure.DI;
using HotelManagement.Persistence.DI;
using HotelManagement.API.DI;

namespace HotelManagement.API.Extensions
{
    public static class AppExtensions
    {
        public static WebApplicationBuilder AddApplication(this WebApplicationBuilder builder)
        {
            builder.Services
                .AddApplication()
                .AddInfrastructure()
                .AddPersistence()
                .AddPresentation();

            return builder;
        }

        public static WebApplication UseApplication(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            // Подключение endpoints
            // app.UseXXXEdpoits();
            app.MapEndpoints();

            return app;
        }

        private static IServiceCollection AddAEndPo
    }
}
