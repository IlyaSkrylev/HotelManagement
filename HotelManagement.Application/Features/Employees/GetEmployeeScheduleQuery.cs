using HotelManagement.Application.Abstractions;
using HotelManagement.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.Employees;

public record GetEmployeeScheduleQuery(
    long EmployeeId,
    int Year,
    int Month) : IRequest<EmployeeScheduleDto>;

public class GetEmployeeScheduleQueryHandler : IRequestHandler<GetEmployeeScheduleQuery, EmployeeScheduleDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetEmployeeScheduleQueryHandler> _logger;

    public GetEmployeeScheduleQueryHandler(IApplicationDbContext context, ILogger<GetEmployeeScheduleQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<EmployeeScheduleDto> Handle(GetEmployeeScheduleQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Запрос графика работы для сотрудника ID: {EmployeeId}, Год: {Year}, Месяц: {Month}",
            request.EmployeeId, request.Year, request.Month);

        var employee = await _context.Employees
            .Include(e => e.ShiftType)
            .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, cancellationToken);

        if (employee == null)
        {
            throw new Exception("Сотрудник не найден");
        }

        var startOfMonth = new DateTimeOffset(request.Year, request.Month, 1, 0, 0, 0, TimeSpan.Zero);
        var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
        var today = DateTimeOffset.UtcNow.Date;

        var workShifts = await _context.WorkShifts
            .Where(ws => ws.EmployeeId == request.EmployeeId && ws.ShiftDate >= startOfMonth && ws.ShiftDate <= endOfMonth)
            .ToListAsync(cancellationToken);

        var shiftsByDate = workShifts.ToDictionary(
            ws => ws.ShiftDate.Date,
            ws => ws
        );

        var schedule = new List<DailyScheduleDto>();

        var shiftType = employee.ShiftType;
        var cycleStartDate = employee.ShiftCycleStartDate.Date;
        var workingDays = shiftType.WorkingDayShifts;
        var workingNights = shiftType.WorkingNightShifts;
        var restDays = shiftType.RestDays;
        var startsWithDay = employee.ShiftCycleStartsWithDay;

        var cyclePattern = new List<string>();

        for (int i = 0; i < workingDays; i++)
            cyclePattern.Add("day");

        for (int i = 0; i < workingNights; i++)
            cyclePattern.Add("night");

        for (int i = 0; i < restDays; i++)
            cyclePattern.Add("rest");

        if (!startsWithDay)
        {
            var dayShifts = cyclePattern.Where(x => x == "day").ToList();
            var nightShifts = cyclePattern.Where(x => x == "night").ToList();
            var restShifts = cyclePattern.Where(x => x == "rest").ToList();

            cyclePattern.Clear();
            cyclePattern.AddRange(nightShifts);
            cyclePattern.AddRange(dayShifts);
            cyclePattern.AddRange(restShifts);
        }

        var currentDate = startOfMonth.Date;
        var endDate = endOfMonth.Date;

        while (currentDate <= endDate)
        {
            bool isOnVacation = false;
            string vacationType = null;

            if (employee.VacationStartDate.HasValue && employee.VacationEndDate.HasValue)
            {
                if (currentDate >= employee.VacationStartDate.Value.Date && currentDate <= employee.VacationEndDate.Value.Date)
                {
                    isOnVacation = true;
                    vacationType = employee.VacationType;
                }
            }

            if (isOnVacation)
            {
                schedule.Add(new DailyScheduleDto
                {
                    Date = currentDate,
                    ShiftType = "vacation",
                    ShiftTypeCode = "vacation",
                    StartTime = null,
                    EndTime = null,
                    IsOnVacation = true,
                    VacationType = vacationType
                });
            }
            else if (shiftsByDate.TryGetValue(currentDate, out var actualShift))
            {
                schedule.Add(new DailyScheduleDto
                {
                    Date = currentDate,
                    ShiftType = "actual",
                    ShiftTypeCode = "actual",
                    StartTime = actualShift.StartTime.HasValue
                        ? TimeOnly.FromDateTime(actualShift.StartTime.Value.UtcDateTime.AddHours(3))
                        : (TimeOnly?)null,
                    EndTime = actualShift.EndTime.HasValue
                        ? TimeOnly.FromDateTime(actualShift.EndTime.Value.UtcDateTime.AddHours(3))
                        : (TimeOnly?)null,
                    IsOnVacation = false,
                    VacationType = null
                });
            }
            else if (currentDate >= today)
            {
                var daysSinceCycleStart = (int)(currentDate - cycleStartDate).TotalDays;
                var cycleIndex = daysSinceCycleStart % cyclePattern.Count;
                var shiftTypeCode = cyclePattern[cycleIndex];

                TimeOnly? startTime = null;
                TimeOnly? endTime = null;

                if (shiftTypeCode == "day")
                {
                    startTime = shiftType.DayShiftStartTime;
                    endTime = shiftType.DayShiftEndTime;
                }
                else if (shiftTypeCode == "night")
                {
                    startTime = shiftType.NightShiftStartTime;
                    endTime = shiftType.NightShiftEndTime;
                }

                schedule.Add(new DailyScheduleDto
                {
                    Date = currentDate,
                    ShiftType = shiftTypeCode,
                    ShiftTypeCode = shiftTypeCode,
                    StartTime = startTime,
                    EndTime = endTime,
                    IsOnVacation = false,
                    VacationType = null
                });
            }
            else
            {
                schedule.Add(new DailyScheduleDto
                {
                    Date = currentDate,
                    ShiftType = "rest",
                    ShiftTypeCode = "rest",
                    StartTime = null,
                    EndTime = null,
                    IsOnVacation = false,
                    VacationType = null
                });
            }

            currentDate = currentDate.AddDays(1);
        }

        return new EmployeeScheduleDto
        {
            EmployeeId = request.EmployeeId,
            Year = request.Year,
            Month = request.Month,
            Schedule = schedule,
            ShiftType = new ShiftTypeInfoDto
            {
                Id = shiftType.Id,
                Name = shiftType.Name,
                DayShiftStart = shiftType.DayShiftStartTime.ToString(@"HH\:mm"),
                DayShiftEnd = shiftType.DayShiftEndTime.ToString(@"HH\:mm"),
                NightShiftStart = shiftType.NightShiftStartTime.ToString(@"HH\:mm"),
                NightShiftEnd = shiftType.NightShiftEndTime.ToString(@"HH\:mm")
            },
            VacationStartDate = employee.VacationStartDate,
            VacationEndDate = employee.VacationEndDate,
            VacationType = employee.VacationType
        };
    }
}