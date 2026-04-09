using HotelManagement.API.Common;
using HotelManagement.API.DI;
using HotelManagement.Application.DI;
using HotelManagement.Application.Settings;
using HotelManagement.Infrastructure.DI;
using HotelManagement.Persistence.DI;
using Microsoft.Extensions.FileProviders;

namespace HotelManagement.API.Extensions;

public static class AppExtensions
{
    public static WebApplicationBuilder AddApplication(this WebApplicationBuilder builder)
    {
        var serverSettings = builder.Configuration.GetSection(ServerSettings.SectionName).Get<ServerSettings>();

        builder.WebHost.UseUrls($"http://{serverSettings?.Host ?? "0.0.0.0"}:{serverSettings?.Port ?? 5030}");

        builder.Services
            .AddAuthorization()
            .AddApplication()
            .AddInfrastructure(builder.Configuration)
            .AddPersistence()
            .AddPresentation()
            .AddSwaggerServices();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecific", policy =>
            {
                if (serverSettings?.AllowedOrigins != null && serverSettings.AllowedOrigins.Any())
                {
                    policy.WithOrigins(serverSettings.AllowedOrigins.ToArray())
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                }
                else
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                }
            });
        });

        return builder;
    }

    public static WebApplication UseApplication(this WebApplication app)
    {
        var serverSettings = app.Configuration.GetSection(ServerSettings.SectionName).Get<ServerSettings>();

        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        if (!Directory.Exists(uploadsPath))
            Directory.CreateDirectory(uploadsPath);

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(uploadsPath),
            RequestPath = "/uploads"
        });

        app.UseCors("AllowSpecific");

        app.UseAuthentication();
        app.UseAuthorization();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

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