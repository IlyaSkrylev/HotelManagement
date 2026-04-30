using HotelManagement.API.Common;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using HotelManagement.Application.Features.HotelStructure;
using MediatR;

namespace HotelManagement.API.Features.HotelStructure;

public class GetFloorsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/hotels/{hotelId}/structure/floors", async (
                long hotelId,
                int page = 1,
                int pageSize = 20,
                IMediator mediator = null!,
                ILogger<GetFloorsEndpoint> logger = null!) =>
        {
            logger.LogInformation("GET /api/hotels/{HotelId}/structure/floors вызван", hotelId);
            var query = new GetFloorsQuery(hotelId, page, pageSize);
            var result = await mediator.Send(query);
            return Results.Ok(BaseResponse.Ok(result));
        })
        .WithName("GetHotelFloors")
        .WithDescription("Получение списка этажей отеля с пагинацией")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}