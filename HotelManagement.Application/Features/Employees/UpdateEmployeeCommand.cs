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
    int TotalCycleDays,
    DateTimeOffset? VacationStartDate = null,
    DateTimeOffset? VacationEndDate = null,
    string? VacationType = null) : IRequest<EmployeeDto>;

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
        _logger.LogInformation("Обновление сотрудника ID: {Id}, RoleId: {RoleId}", request.Id, request.RoleId);

        var employee = await _context.Employees
            .Include(e => e.User)
            .Include(e => e.ShiftType)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (employee == null)
        {
            throw new Exception("Сотрудник не найден");
        }

        var currentUserHotelRole = await _context.UserHotelRoles
            .FirstOrDefaultAsync(uhr => uhr.UserId == employee.UserId && uhr.HotelId == employee.HotelId, cancellationToken);

        long? oldRoleId = currentUserHotelRole?.RoleId;
        var oldRole = oldRoleId.HasValue
            ? await _context.UserRoles.FirstOrDefaultAsync(r => r.Id == oldRoleId.Value, cancellationToken)
            : null;

        var newRole = await _context.UserRoles
            .FirstOrDefaultAsync(r => r.Id == request.RoleId, cancellationToken);

        if (oldRole?.Code == "manager" && newRole?.Code != "manager")
        {
            var departmentsWhereManager = await _context.Departments
                .Where(d => d.ManagerId == employee.Id)
                .ToListAsync(cancellationToken);

            foreach (var department in departmentsWhereManager)
            {
                department.ManagerId = null;
                department.UpdatedAt = DateTimeOffset.UtcNow;
                _logger.LogInformation("Обнулён ManagerId в отделе ID: {DepartmentId} при смене роли сотрудника с manager на {NewRole}",
                    department.Id, newRole?.Code ?? "unknown");
            }
        }

        long? finalDepartmentId = request.DepartmentId;

        if (newRole?.Code == "admin")
        {
            var adminDepartment = await _context.Departments
                .FirstOrDefaultAsync(d => d.HotelId == employee.HotelId && d.Name == "Администрация", cancellationToken);

            if (adminDepartment != null)
            {
                finalDepartmentId = adminDepartment.Id;
                _logger.LogInformation("Администратору назначен отдел 'Администрация' ID: {DepartmentId}", adminDepartment.Id);
            }
            else
            {
                var newAdminDepartment = new Department
                {
                    Name = "Администрация",
                    HotelId = employee.HotelId,
                    Description = "Отдел администрации гостиницы",
                    CreatedAt = DateTimeOffset.UtcNow,
                    UpdatedAt = DateTimeOffset.UtcNow
                };
                _context.Departments.Add(newAdminDepartment);
                await _context.SaveChangesAsync(cancellationToken);
                finalDepartmentId = newAdminDepartment.Id;
                _logger.LogInformation("Создан отдел 'Администрация' ID: {DepartmentId}", newAdminDepartment.Id);
            }
        }
        else if ((newRole?.Code == "manager" || newRole?.Code == "employee") && !finalDepartmentId.HasValue)
        {
            throw new Exception("Для роли 'Менеджер' или 'Сотрудник' необходимо выбрать отдел");
        }

        if (finalDepartmentId.HasValue && employee.DepartmentId != finalDepartmentId.Value)
        {
            var departmentExists = await _context.Departments
                .AnyAsync(d => d.Id == finalDepartmentId.Value, cancellationToken);

            if (!departmentExists)
            {
                throw new Exception($"Отдел с ID {finalDepartmentId.Value} не найден");
            }

            employee.DepartmentId = finalDepartmentId.Value;
            _logger.LogInformation("Отдел сотрудника изменен на ID: {DepartmentId}", finalDepartmentId.Value);
        }

        employee.Position = request.Position;
        employee.Salary = (int?)request.Salary;
        employee.SalarySupplement = (int?)request.SalarySupplement;
        employee.ShiftCycleStartDate = request.ShiftCycleStartDate.ToUniversalTime();
        employee.ShiftCycleStartsWithDay = request.ShiftCycleStartsWithDay;
        employee.UpdatedAt = DateTimeOffset.UtcNow;

        employee.VacationStartDate = request.VacationStartDate?.ToUniversalTime();
        employee.VacationEndDate = request.VacationEndDate?.ToUniversalTime();
        employee.VacationType = request.VacationType;

        if (employee.ShiftType != null)
        {
            employee.ShiftType.WorkingDayShifts = request.WorkingDayShifts;
            employee.ShiftType.WorkingNightShifts = request.WorkingNightShifts;
            employee.ShiftType.RestDays = request.RestDays;
            employee.ShiftType.TotalCycleDays = request.TotalCycleDays;
            employee.ShiftType.DayShiftStartTime = request.DayShiftStart;
            employee.ShiftType.DayShiftEndTime = request.DayShiftEnd;
            employee.ShiftType.NightShiftStartTime = request.NightShiftStart;
            employee.ShiftType.NightShiftEndTime = request.NightShiftEnd;
        }
        else
        {
            var guid = Guid.NewGuid().ToString("N");
            var code = $"CUSTOM_{guid}";
            if (code.Length > 50) code = code.Substring(0, 50);

            var newShiftType = new ShiftType
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
            _context.ShiftTypes.Add(newShiftType);
            await _context.SaveChangesAsync(cancellationToken);
            employee.ShiftTypeId = newShiftType.Id;
        }

        await _context.SaveChangesAsync(cancellationToken);

        if (currentUserHotelRole != null)
        {
            currentUserHotelRole.RoleId = request.RoleId;
            await _context.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation("Сотрудник ID: {Id} успешно обновлен. DepartmentId: {DepartmentId}, RoleId: {RoleId}",
            request.Id, employee.DepartmentId, request.RoleId);

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
}