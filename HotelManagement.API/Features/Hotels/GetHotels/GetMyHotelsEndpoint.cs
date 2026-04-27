using HotelManagement.API.Common;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using HotelManagement.Application.Features.Hotels;
using MediatR;

namespace HotelManagement.API.Features.Hotels.GetMyHotels;

public class GetMyHotelsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/hotels/myhotels", async (
            [AsParameters] GetMyHotelsQuery query,
            IMediator mediator,
            ILogger<GetMyHotelsEndpoint> logger) =>
        {
            logger.LogInformation("GET /api/hotels/myhotels вызван");
            var result = await mediator.Send(query);
            return Results.Ok(BaseResponse.Ok(result));
        })
            .WithName("GetMyHotels")
            .WithDescription("Возвращает список отелей, где работает текущий пользователь")
            .Produces<PaginatedResult<MyHotelDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();
    }
}