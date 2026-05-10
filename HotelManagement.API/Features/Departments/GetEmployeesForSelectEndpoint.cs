using HotelManagement.API.Common;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using HotelManagement.Application.Features.Departments;
using MediatR;

namespace HotelManagement.API.Features.Departments;

public class GetEmployeesForSelectEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/hotels/{hotelId}/departments/employees", async (
                long hotelId,
                string? searchTerm,
                string? roleCode,
                IMediator mediator = null!,
                ILogger<GetEmployeesForSelectEndpoint> logger = null!) =>
        {
            logger.LogInformation("GET /api/hotels/{HotelId}/departments/employees вызван с roleCode={RoleCode}", hotelId, roleCode);

            var query = new GetHotelEmployeesForSelectQuery(hotelId, searchTerm, roleCode);
            var result = await mediator.Send(query);
            return Results.Ok(BaseResponse.Ok(result));
        })
        .WithName("GetHotelEmployeesForSelect")
        .WithDescription("Получение списка сотрудников отеля для выпадающего списка с фильтрацией по роли")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}