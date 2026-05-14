using HotelManagement.Application.Abstractions;
using HotelManagement.Application.DTOs;
using HotelManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.Departments;

public record GetDepartmentScheduleQuery(
    long DepartmentId,
    int Year,
    int Month) : IRequest<DepartmentScheduleDto>;

public class GetDepartmentScheduleQueryHandler : IRequestHandler<GetDepartmentScheduleQuery, DepartmentScheduleDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetDepartmentScheduleQueryHandler> _logger;

    public GetDepartmentScheduleQueryHandler(IApplicationDbContext context, ILogger<GetDepartmentScheduleQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<DepartmentScheduleDto> Handle(GetDepartmentScheduleQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Запрос графика отдела ID: {DepartmentId}, Год: {Year}, Месяц: {Month}",
            request.DepartmentId, request.Year, request.Month);

        var department = await _context.Departments
            .FirstOrDefaultAsync(d => d.Id == request.DepartmentId, cancellationToken);

        if (department == null)
        {
            throw new Exception("Отдел не найден");
        }

        var employees = await _context.Employees
            .Include(e => e.User)
            .Include(e => e.ShiftType)
            .Where(e => e.DepartmentId == request.DepartmentId && e.IsActive)
            .ToListAsync(cancellationToken);

        var startOfMonth = new DateTimeOffset(request.Year, request.Month, 1, 0, 0, 0, TimeSpan.Zero);
        var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
        var today = DateTimeOffset.UtcNow.Date;

        var workShifts = await _context.WorkShifts
            .Where(ws => employees.Select(e => e.Id).Contains(ws.EmployeeId) &&
                         ws.ShiftDate >= startOfMonth && ws.ShiftDate <= endOfMonth)
            .ToListAsync(cancellationToken);

        var shiftsByEmployeeAndDate = new Dictionary<(long EmployeeId, DateTime Date), WorkShift>();
        foreach (var ws in workShifts)
        {
            var key = (ws.EmployeeId, ws.ShiftDate.Date);
            if (!shiftsByEmployeeAndDate.ContainsKey(key))
            {
                shiftsByEmployeeAndDate.Add(key, ws);
            }
        }

        var schedule = new List<DailyDepartmentScheduleDto>();
        var currentDate = startOfMonth.Date;
        var endDate = endOfMonth.Date;

        while (currentDate <= endDate)
        {
            var daySchedule = new DailyDepartmentScheduleDto
            {
                Date = currentDate,
                Employees = new List<EmployeeScheduleInfoDto>()
            };

            foreach (var employee in employees)
            {
                bool isOnVacation = false;
                if (employee.VacationStartDate.HasValue && employee.VacationEndDate.HasValue)
                {
                    if (currentDate >= employee.VacationStartDate.Value.Date && currentDate <= employee.VacationEndDate.Value.Date)
                    {
                        isOnVacation = true;
                    }
                }

                if (isOnVacation)
                {
                    continue; 
                }

                EmployeeScheduleInfoDto? employeeSchedule = null;

                var key = (employee.Id, currentDate);
                if (shiftsByEmployeeAndDate.TryGetValue(key, out var actualShift))
                {
                    employeeSchedule = new EmployeeScheduleInfoDto
                    {
                        EmployeeId = employee.Id,
                        UserId = employee.UserId,
                        FullName = $"{employee.User.LastName} {employee.User.FirstName} {employee.User.Patronymic}".Trim(),
                        AvatarUrl = employee.User.AvatarUrl,
                        Position = employee.Position,
                        ShiftType = "actual",
                        ShiftTypeName = "Фактическая смена",
                        StartTime = actualShift.StartTime.HasValue
                            ? actualShift.StartTime.Value.UtcDateTime.AddHours(3).ToString(@"HH\:mm")
                            : null,
                        EndTime = actualShift.EndTime.HasValue
                            ? actualShift.EndTime.Value.UtcDateTime.AddHours(3).ToString(@"HH\:mm")
                            : null
                    };
                }
                else if (currentDate > today)
                {
                    var shiftType = employee.ShiftType;
                    var cycleStartDate = employee.ShiftCycleStartDate.Date;
                    var cycleDays = shiftType.TotalCycleDays;
                    var workingDays = shiftType.WorkingDayShifts;
                    var workingNights = shiftType.WorkingNightShifts;

                    var daysSinceCycleStart = (int)(currentDate - cycleStartDate).TotalDays;
                    var cycleIndex = daysSinceCycleStart % cycleDays;

                    if (cycleIndex < workingDays)
                    {
                        employeeSchedule = new EmployeeScheduleInfoDto
                        {
                            EmployeeId = employee.Id,
                            UserId = employee.UserId,
                            FullName = $"{employee.User.LastName} {employee.User.FirstName} {employee.User.Patronymic}".Trim(),
                            AvatarUrl = employee.User.AvatarUrl,
                            Position = employee.Position,
                            ShiftType = "day",
                            ShiftTypeName = "Дневная смена",
                            StartTime = shiftType.DayShiftStartTime.ToString(@"HH\:mm"),
                            EndTime = shiftType.DayShiftEndTime.ToString(@"HH\:mm")
                        };
                    }
                    else if (cycleIndex < workingDays + workingNights)
                    {
                        employeeSchedule = new EmployeeScheduleInfoDto
                        {
                            EmployeeId = employee.Id,
                            UserId = employee.UserId,
                            FullName = $"{employee.User.LastName} {employee.User.FirstName} {employee.User.Patronymic}".Trim(),
                            AvatarUrl = employee.User.AvatarUrl,
                            Position = employee.Position,
                            ShiftType = "night",
                            ShiftTypeName = "Ночная смена",
                            StartTime = shiftType.NightShiftStartTime.ToString(@"HH\:mm"),
                            EndTime = shiftType.NightShiftEndTime.ToString(@"HH\:mm")
                        };
                    }
                }

                if (employeeSchedule != null)
                {
                    daySchedule.Employees.Add(employeeSchedule);
                }
            }

            schedule.Add(daySchedule);
            currentDate = currentDate.AddDays(1);
        }

        return new DepartmentScheduleDto
        {
            DepartmentId = request.DepartmentId,
            DepartmentName = department.Name,
            Year = request.Year,
            Month = request.Month,
            Schedule = schedule,
            TotalEmployees = employees.Count
        };
    }
}