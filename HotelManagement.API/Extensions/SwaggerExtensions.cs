using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models; 

namespace HotelManagement.API.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Hotel Management API",
                Version = "v1",
                Description = "API для управления персоналом гостиничного комплекса"
            });
        });

        return services;
    }

    public static WebApplication UseSwaggerUi(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hotel Management API v1");
            });
        }

        return app;
    }
}