using HotelManagement.API.Common;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using HotelManagement.Application.Features.HotelStructure;
using MediatR;

namespace HotelManagement.API.Features.HotelStructure;

public class UpdateRoomEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/hotels/structure/rooms/{id}", async (
                long id,
                UpdateRoomDto dto,
                IMediator mediator = null!,
                ILogger<UpdateRoomEndpoint> logger = null!) =>
        {
            logger.LogInformation("PUT /api/hotels/structure/rooms/{Id} вызван", id);
            var command = new UpdateRoomCommand(id, dto.FloorId, dto.RoomNumber, dto.RoomStatusId, dto.Description);
            var result = await mediator.Send(command);
            return Results.Ok(BaseResponse.Ok(result));
        })
        .WithName("UpdateHotelRoom")
        .WithDescription("Обновление информации о номере")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}