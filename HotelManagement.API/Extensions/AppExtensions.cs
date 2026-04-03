using HotelManagement.API.Common;
using HotelManagement.API.DI;
using HotelManagement.Application.DI;
using HotelManagement.Infrastructure.DI;
using HotelManagement.Persistence.DI;

namespace HotelManagement.API.Extensions
{
    public static class AppExtensions
    {
        public static WebApplicationBuilder AddApplication(this WebApplicationBuilder builder)
        {
            builder.Services
                .AddApplication()
                .AddInfrastructure(builder.Configuration)
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

            app.MapEndpoints();

            return app;
        }

        private static WebApplication MapEndpoints(this WebApplication app)
        {
            var endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();
            foreach (var endpoint in endpoints)
            {
                endpoint.MapEndpoint(app);
            }
            return app;
        }
    }
}
