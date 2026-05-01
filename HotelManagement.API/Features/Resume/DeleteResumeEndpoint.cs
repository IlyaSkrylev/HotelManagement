using HotelManagement.API.Common;
using HotelManagement.Application.Features.Resumes;
using MediatR;

namespace HotelManagement.API.Features.Resumes;

public class DeleteResumeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/resumes/{resumeId}", async (
                long resumeId,
                IMediator mediator,
                ILogger<DeleteResumeEndpoint> logger) =>
        {
            logger.LogInformation("DELETE /api/resumes/{ResumeId} вызван", resumeId);

            var command = new DeleteResumeCommand(resumeId);
            var result = await mediator.Send(command);

            if (result)
                return Results.Ok(BaseResponse.Ok(null, "Резюме успешно удалено"));

            return Results.NotFound(BaseResponse.Error("Резюме не найдено"));
        })
        .WithName("DeleteResume")
        .WithDescription("Удаление резюме из системы")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .RequireAuthorization();
    }
}