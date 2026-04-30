using HotelManagement.API.Common;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using HotelManagement.Application.Features.HotelStructure;
using MediatR;

namespace HotelManagement.API.Features.HotelStructure;

public class CreateFloorEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/hotels/{hotelId}/structure/floors", async (
                long hotelId,
                CreateFloorDto dto,
                IMediator mediator = null!,
                ILogger<CreateFloorEndpoint> logger = null!) =>
        {
            logger.LogInformation("POST /api/hotels/{HotelId}/structure/floors вызван", hotelId);
            var command = new CreateFloorCommand(hotelId, dto.FloorNumber, dto.Name, dto.Description);
            var result = await mediator.Send(command);
            return Results.Ok(BaseResponse.Ok(result));
        })
        .WithName("CreateHotelFloor")
        .WithDescription("Создание нового этажа в отеле")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}