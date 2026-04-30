using HotelManagement.API.Common;
using HotelManagement.Application.Common;
using HotelManagement.Application.Features.HotelStructure;
using MediatR;

namespace HotelManagement.API.Features.HotelStructure;

public class DeleteRoomEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/hotels/structure/rooms/{id}", async (
                long id,
                IMediator mediator = null!,
                ILogger<DeleteRoomEndpoint> logger = null!) =>
        {
            logger.LogInformation("DELETE /api/hotels/structure/rooms/{Id} вызван", id);
            var result = await mediator.Send(new DeleteRoomCommand(id));
            return Results.Ok(BaseResponse.Ok(result));
        })
        .WithName("DeleteHotelRoom")
        .WithDescription("Удаление номера")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}