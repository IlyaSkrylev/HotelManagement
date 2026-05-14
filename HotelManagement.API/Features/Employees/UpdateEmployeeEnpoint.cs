using HotelManagement.API.Common;
using HotelManagement.Application.DTOs;
using HotelManagement.Application.Features.Employees;
using MediatR;

namespace HotelManagement.API.Features.Employees;

public class UpdateEmployeeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/employees/{id}", async (
                long id,
                UpdateEmployeeRequest request,
                IMediator mediator,
                ILogger<UpdateEmployeeEndpoint> logger) =>
        {
            logger.LogInformation("PUT /api/employees/{Id} вызван", id);

            var command = new UpdateEmployeeCommand(
                id,
                request.RoleId,
                request.DepartmentId,
                request.Position,
                request.Salary,
                request.SalarySupplement,
                request.WorkingDayShifts,
                request.WorkingNightShifts,
                request.RestDays,
                TimeOnly.Parse(request.DayShiftStart),
                TimeOnly.Parse(request.DayShiftEnd),
                TimeOnly.Parse(request.NightShiftStart),
                TimeOnly.Parse(request.NightShiftEnd),
                request.ShiftCycleStartsWithDay,
                request.ShiftCycleStartDate,
                request.TotalCycleDays,
                request.VacationStartDate,
                request.VacationEndDate,
                request.VacationType);

            var result = await mediator.Send(command);
            return Results.Ok(BaseResponse.Ok(result));
        })
        .WithName("UpdateEmployee")
        .WithDescription("Обновление информации о сотруднике")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .RequireAuthorization();
    }
}

public class UpdateEmployeeRequest
{
    public long RoleId { get; set; }
    public long? DepartmentId { get; set; }
    public string Position { get; set; } = string.Empty;
    public decimal? Salary { get; set; }
    public decimal? SalarySupplement { get; set; }
    public int WorkingDayShifts { get; set; }
    public int WorkingNightShifts { get; set; }
    public int RestDays { get; set; }
    public string DayShiftStart { get; set; } = "09:00";
    public string DayShiftEnd { get; set; } = "18:00";
    public string NightShiftStart { get; set; } = "21:00";
    public string NightShiftEnd { get; set; } = "06:00";
    public bool ShiftCycleStartsWithDay { get; set; } = true;
    public DateTimeOffset ShiftCycleStartDate { get; set; }
    public int TotalCycleDays { get; set; }
    public DateTimeOffset? VacationStartDate { get; set; }
    public DateTimeOffset? VacationEndDate { get; set; }
    public string? VacationType { get; set; }
}