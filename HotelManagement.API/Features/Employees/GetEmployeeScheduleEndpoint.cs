using HotelManagement.API.Common;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using HotelManagement.Application.Features.Employees;
using MediatR;

namespace HotelManagement.API.Features.Employees;

public class GetEmployeeScheduleEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/employees/{employeeId}/schedule", async (
                long employeeId,
                int year,
                int month,
                IMediator mediator,
                ILogger<GetEmployeeScheduleEndpoint> logger) =>
        {
            logger.LogInformation("GET /api/employees/{EmployeeId}/schedule вызван", employeeId);
            var query = new GetEmployeeScheduleQuery(employeeId, year, month);
            var result = await mediator.Send(query);
            return Results.Ok(BaseResponse.Ok(result));
        })
        .WithName("GetEmployeeSchedule")
        .WithDescription("Получение графика работы сотрудника на месяц")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}