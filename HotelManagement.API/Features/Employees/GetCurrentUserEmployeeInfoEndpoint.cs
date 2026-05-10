using HotelManagement.API.Common;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using HotelManagement.Application.Features.Employees;
using MediatR;

namespace HotelManagement.API.Features.Employees;

public class GetCurrentUserEmployeeInfoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/hotels/{hotelId}/current-employee-info", async (
                long hotelId,
                IMediator mediator,
                ILogger<GetCurrentUserEmployeeInfoEndpoint> logger) =>
        {
            logger.LogInformation("GET /api/hotels/{HotelId}/current-employee-info вызван", hotelId);

            var query = new GetCurrentUserEmployeeInfoQuery(hotelId);
            var result = await mediator.Send(query);

            if (result == null)
            {
                return Results.NotFound(BaseResponse.Error("Сотрудник не найден"));
            }

            return Results.Ok(BaseResponse.Ok(result));
        })
        .WithName("GetCurrentUserEmployeeInfo")
        .WithDescription("Получение информации о текущем пользователе как о сотруднике в отеле")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .RequireAuthorization();
    }
}