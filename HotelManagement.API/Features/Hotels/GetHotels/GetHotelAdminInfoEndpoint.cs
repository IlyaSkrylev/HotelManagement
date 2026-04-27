using HotelManagement.API.Common;
using HotelManagement.Application.Features.Hotels;
using MediatR;

namespace HotelManagement.API.Features.Hotels;

public class GetHotelAdminInfoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/hotels/{hotelId}/admin-info", async (
                long hotelId,
                IMediator mediator,
                ILogger<GetHotelAdminInfoEndpoint> logger) =>
        {
            logger.LogInformation("GET /api/hotels/{HotelId}/admin-info вызван", hotelId);
            var result = await mediator.Send(new GetHotelAdminInfoQuery(hotelId));
            return Results.Ok(BaseResponse.Ok(result));
        })
            .WithName("GetHotelAdminInfo")
            .WithDescription("Получение информации об отеле для администратора")
            .Produces<BaseResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }
}