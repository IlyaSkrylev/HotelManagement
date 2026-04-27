using HotelManagement.API.Common;
using HotelManagement.Application.Features.Profile;
using MediatR;

namespace HotelManagement.API.Features.Profile;

public class UploadResumeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/profile/upload-resume", async (
                HttpRequest request,
                IMediator mediator,
                ILogger<UploadResumeEndpoint> logger) =>
        {
            var form = await request.ReadFormAsync();
            var file = form.Files.GetFile("Resume");

            if (file == null)
                return Results.BadRequest(BaseResponse.Error("Файл не выбран"));

            var command = new UploadResumeCommand(file);
            logger.LogInformation("POST /api/profile/upload-resume вызван");
            var result = await mediator.Send(command);
            return Results.Ok(BaseResponse.Ok(new { Url = result }, "Резюме загружено"));
        })
            .WithName("UploadResume")
            .WithDescription("Загрузка резюме пользователя")
            .Accepts<UploadResumeCommand>("multipart/form-data")
            .Produces<BaseResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();
    }
}