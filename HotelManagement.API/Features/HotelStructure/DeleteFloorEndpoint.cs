using HotelManagement.API.Common;
using HotelManagement.Application.Common;
using HotelManagement.Application.Features.HotelStructure;
using MediatR;

namespace HotelManagement.API.Features.HotelStructure;

public class DeleteFloorEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/hotels/structure/floors/{id}", async (
                long id,
                IMediator mediator = null!,
                ILogger<DeleteFloorEndpoint> logger = null!) =>
        {
            logger.LogInformation("DELETE /api/hotels/structure/floors/{Id} вызван", id);
            var result = await mediator.Send(new DeleteFloorCommand(id));
            return Results.Ok(BaseResponse.Ok(result));
        })
        .WithName("DeleteHotelFloor")
        .WithDescription("Удаление этажа")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}