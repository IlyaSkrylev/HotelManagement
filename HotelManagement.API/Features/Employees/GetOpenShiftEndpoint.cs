using HotelManagement.API.Common;
using HotelManagement.Application.Abstractions;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.API.Features.Employees;

public class GetOpenShiftEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/employees/{employeeId}/schedule/open-shift", async (
                long employeeId,
                IApplicationDbContext context,
                ILogger<GetOpenShiftEndpoint> logger) =>
        {
            logger.LogInformation("GET /api/employees/{EmployeeId}/schedule/open-shift вызван", employeeId);

            var openShift = await context.WorkShifts
                .FirstOrDefaultAsync(ws => ws.EmployeeId == employeeId && ws.EndTime == null);

            if (openShift == null)
                return Results.Ok(BaseResponse.Ok(null));

            return Results.Ok(BaseResponse.Ok(new WorkShiftDto
            {
                Id = openShift.Id,
                EmployeeId = openShift.EmployeeId,
                ShiftDate = openShift.ShiftDate,
                StartTime = openShift.StartTime,
                EndTime = openShift.EndTime
            }));
        })
        .WithName("GetOpenShift")
        .WithDescription("Получение открытой смены сотрудника")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}