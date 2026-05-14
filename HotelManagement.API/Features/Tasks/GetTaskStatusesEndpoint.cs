using HotelManagement.API.Common;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using HotelManagement.Application.Features.Tasks;
using MediatR;

namespace HotelManagement.API.Features.Tasks;

public class GetTaskStatusesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/hotels/tasks/statuses", async (
                IMediator mediator = null!,
                ILogger<GetTaskStatusesEndpoint> logger = null!) =>
        {
            logger.LogInformation("GET /api/hotels/tasks/statuses вызван");
            var query = new GetTaskStatusesQuery();
            var result = await mediator.Send(query);
            return Results.Ok(BaseResponse.Ok(result));
        })
        .WithName("GetTaskStatuses")
        .WithDescription("Получение списка статусов задач")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}