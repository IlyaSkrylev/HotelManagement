using HotelManagement.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.Employees;

public record FireEmployeeCommand(long EmployeeId, string? DismissalReason) : IRequest<bool>;

public class FireEmployeeCommandHandler : IRequestHandler<FireEmployeeCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<FireEmployeeCommandHandler> _logger;

    public FireEmployeeCommandHandler(IApplicationDbContext context, ILogger<FireEmployeeCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(FireEmployeeCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Увольнение сотрудника ID: {EmployeeId}", request.EmployeeId);

        var employee = await _context.Employees
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, cancellationToken);

        if (employee == null)
        {
            _logger.LogWarning("Сотрудник с ID {EmployeeId} не найден", request.EmployeeId);
            return false;
        }

        employee.IsActive = false;
        employee.DismissalDate = DateTimeOffset.UtcNow;
        employee.DismissalReason = request.DismissalReason;
        employee.UpdatedAt = DateTimeOffset.UtcNow;

        var departmentsWhereManager = await _context.Departments
            .Where(d => d.ManagerId == employee.Id)
            .ToListAsync(cancellationToken);

        foreach (var department in departmentsWhereManager)
        {
            department.ManagerId = null;
            department.UpdatedAt = DateTimeOffset.UtcNow;
            _logger.LogInformation("Обнулён ManagerId в отделе ID: {DepartmentId} при увольнении руководителя отдела", department.Id);
        }

        var userHotelRole = await _context.UserHotelRoles
            .FirstOrDefaultAsync(uhr => uhr.UserId == employee.UserId && uhr.HotelId == employee.HotelId, cancellationToken);

        if (userHotelRole != null)
        {
            _context.UserHotelRoles.Remove(userHotelRole);
            _logger.LogInformation("Удалена роль пользователя {UserId} в отеле {HotelId} из UserHotelRole",
                employee.UserId, employee.HotelId);
        }
        else
        {
            _logger.LogWarning("Запись в UserHotelRole не найдена для UserId {UserId} и HotelId {HotelId}",
                employee.UserId, employee.HotelId);
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Сотрудник ID: {EmployeeId} успешно уволен. IsActive = false, роль удалена", request.EmployeeId);

        return true;
    }
}