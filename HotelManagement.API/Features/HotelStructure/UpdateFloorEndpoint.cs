using HotelManagement.API.Common;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using HotelManagement.Application.Features.HotelStructure;
using MediatR;

namespace HotelManagement.API.Features.HotelStructure;

public class UpdateFloorEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/hotels/structure/floors/{id}", async (
                long id,
                UpdateFloorDto dto,
                IMediator mediator = null!,
                ILogger<UpdateFloorEndpoint> logger = null!) =>
        {
            logger.LogInformation("PUT /api/hotels/structure/floors/{Id} вызван", id);
            var command = new UpdateFloorCommand(id, dto.FloorNumber, dto.Name, dto.Description);
            var result = await mediator.Send(command);
            return Results.Ok(BaseResponse.Ok(result));
        })
        .WithName("UpdateHotelFloor")
        .WithDescription("Обновление информации об этаже")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}