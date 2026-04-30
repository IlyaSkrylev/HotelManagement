using HotelManagement.API.Common;
using HotelManagement.Application.Features.Resumes;
using MediatR;

namespace HotelManagement.API.Features.Resumes;

public class GetResumeStatusesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/resume-statuses", async (
                IMediator mediator,
                ILogger<GetResumeStatusesEndpoint> logger) =>
        {
            logger.LogInformation("GET /api/resume-statuses вызван");
            var result = await mediator.Send(new GetResumeStatusesQuery());
            return Results.Ok(BaseResponse.Ok(result));
        })
            .WithName("GetResumeStatuses")
            .WithDescription("Получение списка статусов резюме")
            .Produces<BaseResponse>(StatusCodes.Status200OK)
            .RequireAuthorization();
    }
}