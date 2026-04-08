using HotelManagement.API.Common;
using HotelManagement.API.DI;
using HotelManagement.Application.DI;
using HotelManagement.Infrastructure.DI;
using HotelManagement.Persistence.DI;

namespace HotelManagement.API.Extensions;

public static class AppExtensions
{
    public static WebApplicationBuilder AddApplication(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddApplication()
            .AddInfrastructure(builder.Configuration) 
            .AddPersistence()
            .AddPresentation()
            .AddSwaggerServices()
            .AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.WithOrigins(
                            "http://localhost:5173",
                            "http://192.168.0.143:5173"
                        )
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });

        return builder;
    } 

    public static WebApplication UseApplication(this WebApplication app) 
    {
        if (app.Environment.IsDevelopment()) 
        {
            app.UseSwagger();      
            app.UseSwaggerUI();
        }

        app.UseCors("AllowAll");

        app.MapEndpoints();

        return app;
    }

    private static WebApplication MapEndpoints(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var endpoints = scope.ServiceProvider.GetRequiredService<IEnumerable<IEndpoint>>();
            foreach (var endpoint in endpoints)
            {
                endpoint.MapEndpoint(app);
            }
        }
        return app;
    }
}