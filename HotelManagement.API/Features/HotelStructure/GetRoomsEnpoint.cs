using HotelManagement.API.Common;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using HotelManagement.Application.Features.HotelStructure;
using MediatR;

namespace HotelManagement.API.Features.HotelStructure;

public class GetRoomsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/hotels/{hotelId}/structure/rooms", async (
                long hotelId,
                long? floorId,
                int page = 1,
                int pageSize = 20,
                IMediator mediator = null!,
                ILogger<GetRoomsEndpoint> logger = null!) =>
        {
            logger.LogInformation("GET /api/hotels/{HotelId}/structure/rooms вызван", hotelId);
            var query = new GetRoomsQuery(hotelId, floorId, page, pageSize);
            var result = await mediator.Send(query);
            return Results.Ok(BaseResponse.Ok(result));
        })
        .WithName("GetHotelRooms")
        .WithDescription("Получение списка номеров отеля с пагинацией и фильтрацией по этажу")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}