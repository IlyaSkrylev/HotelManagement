using HotelManagement.Application.Abstractions;
using HotelManagement.Application.DTOs;
using HotelManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.Employees;

public record StartWorkShiftCommand(long EmployeeId) : IRequest<WorkShiftDto>;

public class StartWorkShiftCommandHandler : IRequestHandler<StartWorkShiftCommand, WorkShiftDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<StartWorkShiftCommandHandler> _logger;

    public StartWorkShiftCommandHandler(IApplicationDbContext context, ILogger<StartWorkShiftCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<WorkShiftDto> Handle(StartWorkShiftCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Начало смены для сотрудника ID: {EmployeeId}", request.EmployeeId);

        var today = DateTimeOffset.UtcNow.Date;

        var openShift = await _context.WorkShifts
            .FirstOrDefaultAsync(ws => ws.EmployeeId == request.EmployeeId && ws.EndTime == null, cancellationToken);

        if (openShift != null)
        {
            throw new Exception("У вас уже есть незакрытая смена. Закройте её сначала.");
        }

        var todayShift = await _context.WorkShifts
            .FirstOrDefaultAsync(ws => ws.EmployeeId == request.EmployeeId && ws.ShiftDate.Date == today, cancellationToken);

        if (todayShift != null)
        {
            throw new Exception("Сегодня вы уже отмечали смену. Новая смена может быть начата только на следующий день.");
        }

        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == request.EmployeeId && e.IsActive, cancellationToken); // Добавлена проверка IsActive

        if (employee == null)
        {
            throw new Exception("Активный сотрудник не найден");
        }

        var shiftTypeExists = await _context.ShiftTypes
            .AnyAsync(st => st.Id == employee.ShiftTypeId, cancellationToken);

        if (!shiftTypeExists)
        {
            throw new Exception($"Тип смены с ID {employee.ShiftTypeId} не найден");
        }

        var now = DateTimeOffset.UtcNow;

        var shift = new WorkShift
        {
            EmployeeId = request.EmployeeId,
            ShiftTypeId = employee.ShiftTypeId,
            ShiftDate = now,
            StartTime = now,
            EndTime = null
        };

        _context.WorkShifts.Add(shift);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Смена начата для сотрудника ID: {EmployeeId}, ShiftId: {ShiftId}", request.EmployeeId, shift.Id);

        return new WorkShiftDto
        {
            Id = shift.Id,
            EmployeeId = shift.EmployeeId,
            ShiftDate = shift.ShiftDate,
            StartTime = shift.StartTime,
            EndTime = shift.EndTime
        };
    }
}