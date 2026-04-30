using HotelManagement.API.Common;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using HotelManagement.Application.Features.HotelStructure;
using MediatR;

namespace HotelManagement.API.Features.HotelStructure;

public class GetRoomStatusesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/hotels/structure/room-statuses", async (
                IMediator mediator = null!,
                ILogger<GetRoomStatusesEndpoint> logger = null!) =>
        {
            logger.LogInformation("GET /api/hotels/structure/room-statuses вызван");
            var query = new GetRoomStatusesQuery();
            var result = await mediator.Send(query);
            return Results.Ok(BaseResponse.Ok(result));
        })
        .WithName("GetRoomStatuses")
        .WithDescription("Получение списка статусов номеров")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}