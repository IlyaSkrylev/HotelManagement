using HotelManagement.API.Extensions;
using HotelManagement.API.Middleware;
using HotelManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();

builder.AddApplication();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseApplication();
app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

app.Run();