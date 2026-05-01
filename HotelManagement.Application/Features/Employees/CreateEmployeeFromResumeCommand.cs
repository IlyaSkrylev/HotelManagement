// CreateEmployeeFromResumeCommand.cs - добавьте удаление резюме
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

        // 1. Находим резюме
        var resume = await _context.Resumes
            .FirstOrDefaultAsync(r => r.UserId == request.UserId && r.HotelId == request.HotelId, cancellationToken);

        if (resume == null)
        {
            _logger.LogWarning("Резюме не найдено для UserId {UserId} в отеле {HotelId}", request.UserId, request.HotelId);
            throw new Exception("Резюме не найдено");
        }

        // 2. Создаем тип смены
        var shiftType = await CreateOrGetShiftType(request, cancellationToken);

        // 3. Создаем сотрудника
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
            ShiftCycleStartDate = request.ShiftCycleStartDate.ToUniversalTime(),
            ShiftCycleStartsWithDay = request.ShiftCycleStartsWithDay,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync(cancellationToken);

        // 4. Создаем роль пользователя в отеле
        var userHotelRole = new UserHotelRole
        {
            UserId = request.UserId,
            HotelId = request.HotelId,
            RoleId = request.RoleId,
            AssignedAt = DateTimeOffset.UtcNow
        };

        _context.UserHotelRoles.Add(userHotelRole);
        await _context.SaveChangesAsync(cancellationToken);

        // 5. УДАЛЯЕМ РЕЗЮМЕ (вместо изменения статуса!)
        _context.Resumes.Remove(resume);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Сотрудник создан и резюме удалено. EmployeeId: {EmployeeId}, ResumeId: {ResumeId}",
            employee.Id, resume.Id);

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
        var guid = Guid.NewGuid().ToString("N");
        var code = $"CUSTOM_{guid}";

        if (code.Length > 50)
        {
            code = code.Substring(0, 50);
        }

        var existingCode = await _context.ShiftTypes
            .AnyAsync(st => st.Code == code, cancellationToken);

        if (existingCode)
        {
            code = $"{code}_{DateTime.UtcNow.Ticks}";
            if (code.Length > 50)
            {
                code = code.Substring(0, 50);
            }
        }

        var shiftType = new ShiftType
        {
            Code = code,
            Name = "Индивидуальный график",
            Color = "#3498db",
            Description = $"Рабочих дней: {request.WorkingDayShifts}, ночей: {request.WorkingNightShifts}, отдыха: {request.RestDays}",
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