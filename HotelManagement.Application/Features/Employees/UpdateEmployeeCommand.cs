using HotelManagement.Application.Abstractions;
using HotelManagement.Application.DTOs;
using HotelManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.Employees;

public record UpdateEmployeeCommand(
    long Id,
    long RoleId,
    long? DepartmentId,
    string Position,
    decimal? Salary,
    decimal? SalarySupplement,
    int WorkingDayShifts,
    int WorkingNightShifts,
    int RestDays,
    TimeOnly DayShiftStart,
    TimeOnly DayShiftEnd,
    TimeOnly NightShiftStart,
    TimeOnly NightShiftEnd,
    bool ShiftCycleStartsWithDay,
    DateTimeOffset ShiftCycleStartDate,
    int TotalCycleDays) : IRequest<EmployeeDto>;

public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, EmployeeDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<UpdateEmployeeCommandHandler> _logger;

    public UpdateEmployeeCommandHandler(IApplicationDbContext context, ILogger<UpdateEmployeeCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<EmployeeDto> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Обновление сотрудника ID: {Id}", request.Id);

        var employee = await _context.Employees
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (employee == null)
        {
            throw new Exception("Сотрудник не найден");
        }

        employee.Position = request.Position;
        employee.DepartmentId = request.DepartmentId ?? 0;
        employee.Salary = (int?)request.Salary;
        employee.SalarySupplement = (int?)request.SalarySupplement;
        employee.UpdatedAt = DateTimeOffset.UtcNow;

        var shiftType = await CreateOrUpdateShiftType(employee, request, cancellationToken);
        employee.ShiftTypeId = shiftType.Id;
        employee.ShiftCycleStartDate = request.ShiftCycleStartDate;
        employee.ShiftCycleStartsWithDay = request.ShiftCycleStartsWithDay;

        await _context.SaveChangesAsync(cancellationToken);

        var userHotelRole = await _context.UserHotelRoles
            .FirstOrDefaultAsync(uhr => uhr.UserId == employee.UserId && uhr.HotelId == employee.HotelId, cancellationToken);

        if (userHotelRole != null)
        {
            userHotelRole.RoleId = request.RoleId;
            await _context.SaveChangesAsync(cancellationToken);
        }

        return new EmployeeDto
        {
            Id = employee.Id,
            UserId = employee.UserId,
            Position = employee.Position,
            DepartmentId = employee.DepartmentId,
            IsActive = employee.IsActive,
            HireDate = employee.HireDate
        };
    }

    private async Task<ShiftType> CreateOrUpdateShiftType(Employee employee, UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var existingShiftType = await _context.ShiftTypes
            .FirstOrDefaultAsync(st => st.Id == employee.ShiftTypeId, cancellationToken);

        if (existingShiftType != null)
        {
            existingShiftType.WorkingDayShifts = request.WorkingDayShifts;
            existingShiftType.WorkingNightShifts = request.WorkingNightShifts;
            existingShiftType.RestDays = request.RestDays;
            existingShiftType.TotalCycleDays = request.TotalCycleDays;
            existingShiftType.DayShiftStartTime = request.DayShiftStart;
            existingShiftType.DayShiftEndTime = request.DayShiftEnd;
            existingShiftType.NightShiftStartTime = request.NightShiftStart;
            existingShiftType.NightShiftEndTime = request.NightShiftEnd;

            await _context.SaveChangesAsync(cancellationToken);
            return existingShiftType;
        }

        var code = $"CUSTOM_{Guid.NewGuid():N}".Substring(0, 50);
        var newShiftType = new ShiftType
        {
            Code = code,
            Name = $"Индивидуальный график",
            Color = "#3498db",
            Description = $"Автоматически созданный график",
            TotalCycleDays = request.TotalCycleDays,
            WorkingDayShifts = request.WorkingDayShifts,
            WorkingNightShifts = request.WorkingNightShifts,
            RestDays = request.RestDays,
            DayShiftStartTime = request.DayShiftStart,
            DayShiftEndTime = request.DayShiftEnd,
            NightShiftStartTime = request.NightShiftStart,
            NightShiftEndTime = request.NightShiftEnd
        };

        _context.ShiftTypes.Add(newShiftType);
        await _context.SaveChangesAsync(cancellationToken);

        return newShiftType;
    }
}