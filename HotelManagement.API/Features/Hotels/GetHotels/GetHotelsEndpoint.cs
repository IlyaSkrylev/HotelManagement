using HotelManagement.API.Common;
using HotelManagement.Application.Common;
using HotelManagement.Application.Features.Hotels;
using MediatR;

namespace HotelManagement.API.Features.Hotels.GetHotels
{
    public class GetHotelsEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/hotels", async (
                    [AsParameters] GetHotelsQuery query,
                    IMediator mediator,
                    ILogger<GetHotelsEndpoint> logger) =>
                    {
                        logger.LogInformation("GET /api/hotels вызван");
                        var result = await mediator.Send(query);
                        return Results.Ok(result);
                    })
                .WithName("GetHotels")
                .WithDescription("Возвращает список всех гостиниц с пагинацией")
                .Produces<PaginatedResult<HotelDto>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized)
                .RequireAuthorization();
        }
    }
}
