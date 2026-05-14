using HotelManagement.API.Common;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using HotelManagement.Application.Features.Tasks;
using MediatR;

namespace HotelManagement.API.Features.Tasks;

public class GetTaskPrioritiesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/hotels/tasks/priorities", async (
                IMediator mediator = null!,
                ILogger<GetTaskPrioritiesEndpoint> logger = null!) =>
        {
            logger.LogInformation("GET /api/hotels/tasks/priorities вызван");
            var query = new GetTaskPrioritiesQuery();
            var result = await mediator.Send(query);
            return Results.Ok(BaseResponse.Ok(result));
        })
        .WithName("GetTaskPriorities")
        .WithDescription("Получение списка приоритетов задач")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}