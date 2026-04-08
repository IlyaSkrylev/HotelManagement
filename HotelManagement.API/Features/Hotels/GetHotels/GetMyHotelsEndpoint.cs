using HotelManagement.API.Common;
using HotelManagement.Application.Common;
using HotelManagement.Application.Features.Hotels;
using MediatR;

namespace HotelManagement.API.Features.Hotels.GetHotels;

public class GetMyHotelsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/hotels/myhotels/", async (
            [AsParameters] GetMyHotelsQuery query,
            IMediator mediator,
            ILogger<GetMyHotelsEndpoint> logger) =>
            {
                logger.LogInformation("GET api/hotels/myhotels вызван");
                var result = await mediator.Send(query);
                return Results.Ok(result);
            })
            .WithName("myhotles")
            .WithDescription("Возвращает список отелей пользователя с пагинацией")
            .Produces<PaginatedResult<HotelDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();
    }
}
