using HotelManagement.API.Common;
using HotelManagement.Application.DTOs;
using HotelManagement.Application.Features.Employees;
using MediatR;

namespace HotelManagement.API.Features.Employees;

public class HireFromResumeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/hotels/{hotelId}/hire/{userId}", async (
                long hotelId,
                long userId,
                HireFromResumeRequest request,
                IMediator mediator,
                ILogger<HireFromResumeEndpoint> logger) =>
        {
            logger.LogInformation("POST /api/hotels/{HotelId}/hire/{UserId} вызван", hotelId, userId);

            var command = new CreateEmployeeFromResumeCommand(
                hotelId,
                userId,
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
                request.TotalCycleDays);

            var result = await mediator.Send(command);
            return Results.Ok(BaseResponse.Ok(result));
        })
        .WithName("HireFromResume")
        .WithDescription("Наём сотрудника из одобренного резюме")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}

public class HireFromResumeRequest
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
}