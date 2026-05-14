using HotelManagement.API.Common;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using HotelManagement.Application.Features.Departments;
using MediatR;

namespace HotelManagement.API.Features.Departments;

public class GetDepartmentScheduleEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/departments/{departmentId}/schedule", async (
                long departmentId,
                int year,
                int month,
                IMediator mediator,
                ILogger<GetDepartmentScheduleEndpoint> logger) =>
        {
            logger.LogInformation("GET /api/departments/{DepartmentId}/schedule вызван", departmentId);
            var query = new GetDepartmentScheduleQuery(departmentId, year, month);
            var result = await mediator.Send(query);
            return Results.Ok(BaseResponse.Ok(result));
        })
        .WithName("GetDepartmentSchedule")
        .WithDescription("Получение графика работы отдела на месяц")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}