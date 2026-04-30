using HotelManagement.Application.Abstractions;
using HotelManagement.Application.DTOs;
using HotelManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.Employees;

public record CreateEmployeeFromResumeCommand(
    long HotelId,
    long UserId,
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

public class CreateEmployeeFromResumeCommandHandler : IRequestHandler<CreateEmployeeFromResumeCommand, EmployeeDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CreateEmployeeFromResumeCommandHandler> _logger;

    public CreateEmployeeFromResumeCommandHandler(IApplicationDbContext context, ILogger<CreateEmployeeFromResumeCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<EmployeeDto> Handle(CreateEmployeeFromResumeCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Найм сотрудника из резюме для отеля ID: {HotelId}, UserId: {UserId}", request.HotelId, request.UserId);

        var shiftType = await CreateOrGetShiftType(request, cancellationToken);

        var employee = new Employee
        {
            UserId = request.UserId,
            HotelId = request.HotelId,
            DepartmentId = request.DepartmentId ?? 0,
            Position = request.Position,
            HireDate = DateTimeOffset.UtcNow,
            IsActive = true,
            Salary = (int?)request.Salary,
            SalarySupplement = (int?)request.SalarySupplement,
            ShiftTypeId = shiftType.Id,
            ShiftCycleStartDate = request.ShiftCycleStartDate,
            ShiftCycleStartsWithDay = request.ShiftCycleStartsWithDay,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync(cancellationToken);

        var userHotelRole = new UserHotelRole
        {
            UserId = request.UserId,
            HotelId = request.HotelId,
            RoleId = request.RoleId,
            AssignedAt = DateTimeOffset.UtcNow
        };

        _context.UserHotelRoles.Add(userHotelRole);
        await _context.SaveChangesAsync(cancellationToken);

        var resume = await _context.Resumes
            .FirstOrDefaultAsync(r => r.UserId == request.UserId && r.HotelId == request.HotelId, cancellationToken);

        if (resume != null)
        {
            var approvedStatus = await _context.ResumeStatuses
                .FirstOrDefaultAsync(s => s.Code == "APPROVED", cancellationToken);
            if (approvedStatus != null)
            {
                resume.StatusId = approvedStatus.Id;
                await _context.SaveChangesAsync(cancellationToken);
            }
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

    private async Task<ShiftType> CreateOrGetShiftType(CreateEmployeeFromResumeCommand request, CancellationToken cancellationToken)
    {
        var code = $"CUSTOM_{Guid.NewGuid():N}".Substring(0, 50);

        var shiftType = new ShiftType
        {
            Code = code,
            Name = $"Индивидуальный график",
            Color = "#3498db",
            Description = $"Автоматически созданный график: {request.WorkingDayShifts} дней, {request.WorkingNightShifts} ночей, {request.RestDays} отдыха",
            TotalCycleDays = request.TotalCycleDays,
            WorkingDayShifts = request.WorkingDayShifts,
            WorkingNightShifts = request.WorkingNightShifts,
            RestDays = request.RestDays,
            DayShiftStartTime = request.DayShiftStart,
            DayShiftEndTime = request.DayShiftEnd,
            NightShiftStartTime = request.NightShiftStart,
            NightShiftEndTime = request.NightShiftEnd
        };

        _context.ShiftTypes.Add(shiftType);
        await _context.SaveChangesAsync(cancellationToken);

        return shiftType;
    }
}