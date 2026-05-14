using HotelManagement.API.Common;
using HotelManagement.Application.Common;
using HotelManagement.Application.Features.Tasks;
using MediatR;

namespace HotelManagement.API.Features.Tasks;

public class DeleteTaskEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/hotels/tasks/{id}", async (
                long id,
                IMediator mediator = null!,
                ILogger<DeleteTaskEndpoint> logger = null!) =>
        {
            logger.LogInformation("DELETE /api/hotels/tasks/{Id} вызван", id);
            var result = await mediator.Send(new DeleteTaskCommand(id));
            return Results.Ok(BaseResponse.Ok(result));
        })
        .WithName("DeleteTask")
        .WithDescription("Удаление задачи")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}