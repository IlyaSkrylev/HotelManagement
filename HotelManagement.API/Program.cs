using HotelManagement.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddApplication();

builder.Services.AddControllers();

var app = builder.Build();

app.UseApplication();

app.Run();
