using HotelManagement.API.Common;
using HotelManagement.Application.Features.Resumes;
using MediatR;

namespace HotelManagement.API.Features.Resumes;

public class UpdateResumeStatusEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/resumes/{resumeId}/status", async (
                long resumeId,
                UpdateResumeStatusCommand command,
                IMediator mediator,
                ILogger<UpdateResumeStatusEndpoint> logger) =>
        {
            if (resumeId != command.ResumeId)
                return Results.BadRequest(BaseResponse.Error("ID не совпадают"));

            logger.LogInformation("PUT /api/resumes/{ResumeId}/status вызван", resumeId);
            var result = await mediator.Send(command);

            if (result.Success)
                return Results.Ok(BaseResponse.Ok(null, result.Message));

            return Results.BadRequest(BaseResponse.Error(result.Message));
        })
            .WithName("UpdateResumeStatus")
            .WithDescription("Обновление статуса резюме")
            .Accepts<UpdateResumeStatusCommand>("application/json")
            .Produces<BaseResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization();
    }
}