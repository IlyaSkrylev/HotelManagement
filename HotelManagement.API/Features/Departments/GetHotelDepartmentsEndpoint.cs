using HotelManagement.API.Common;
using HotelManagement.Application.Features.Departments;
using MediatR;

namespace HotelManagement.API.Features.Departments;

public class GetHotelDepartmentsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/hotels/{hotelId}/departments", async (
                long hotelId,
                IMediator mediator,
                ILogger<GetHotelDepartmentsEndpoint> logger) =>
        {
            logger.LogInformation("GET /api/hotels/{HotelId}/departments вызван", hotelId);

            var query = new GetHotelDepartmentsQuery(hotelId);
            var result = await mediator.Send(query);

            return Results.Ok(BaseResponse.Ok(result));
        })
        .WithName("GetHotelDepartments")
        .WithDescription("Получение списка отделов отеля")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}