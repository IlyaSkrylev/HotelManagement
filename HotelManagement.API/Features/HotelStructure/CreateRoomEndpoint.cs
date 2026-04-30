using HotelManagement.API.Common;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using HotelManagement.Application.Features.HotelStructure;
using MediatR;

namespace HotelManagement.API.Features.HotelStructure;

public class CreateRoomEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/hotels/{hotelId}/structure/rooms", async (
                long hotelId,
                CreateRoomDto dto,
                IMediator mediator = null!,
                ILogger<CreateRoomEndpoint> logger = null!) =>
        {
            logger.LogInformation("POST /api/hotels/{HotelId}/structure/rooms вызван", hotelId);
            var command = new CreateRoomCommand(hotelId, dto.FloorId, dto.RoomNumber, dto.RoomStatusId, dto.Description);
            var result = await mediator.Send(command);
            return Results.Ok(BaseResponse.Ok(result));
        })
        .WithName("CreateHotelRoom")
        .WithDescription("Создание нового номера в отеле")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}