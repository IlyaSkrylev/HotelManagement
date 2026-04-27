using HotelManagement.API.Common;
using HotelManagement.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.API.Features.Profile;

public class DownloadResumeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/profile/resume", async (
                IApplicationDbContext context,
                ICurrentUserService currentUserService) =>
        {
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Id == currentUserService.UserId);

            if (user == null || string.IsNullOrEmpty(user.ResumeUrl))
                return Results.NotFound("Резюме не найдено");

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), user.ResumeUrl.TrimStart('/'));
            if (!File.Exists(filePath))
                return Results.NotFound("Файл резюме не найден");

            var fileBytes = await File.ReadAllBytesAsync(filePath);
            var contentType = "application/octet-stream";
            var fileName = Path.GetFileName(filePath);

            return Results.File(fileBytes, contentType, fileName);
        })
            .WithName("DownloadResume")
            .WithDescription("Скачивание резюме текущего пользователя")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }
}