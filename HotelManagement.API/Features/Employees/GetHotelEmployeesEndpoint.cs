using HotelManagement.API.Common;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using HotelManagement.Application.Features.Employees;
using MediatR;

namespace HotelManagement.API.Features.Employees;

public class GetHotelEmployeesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/hotels/{hotelId}/employees", async (
                long hotelId,
                string? searchTerm,
                string? departmentName,
                int page = 1,
                int pageSize = 20,
                IMediator mediator = null!,
                ILogger<GetHotelEmployeesEndpoint> logger = null!) =>
        {
            logger.LogInformation("GET /api/hotels/{HotelId}/employees вызван с searchTerm={SearchTerm}, departmentName={DepartmentName}, page={Page}",
                hotelId, searchTerm, departmentName, page);

            var query = new GetHotelEmployeesQuery(hotelId, searchTerm, departmentName, page, pageSize);
            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
            .WithName("GetHotelEmployees")
            .WithDescription("Получение списка сотрудников отеля с пагинацией, поиском и фильтрацией")
            .Produces<PaginatedResult<EmployeeListItemDto>>(StatusCodes.Status200OK)
            .RequireAuthorization();
    }
}