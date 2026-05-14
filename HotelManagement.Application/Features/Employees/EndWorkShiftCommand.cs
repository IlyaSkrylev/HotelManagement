using HotelManagement.Application.Abstractions;
using HotelManagement.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.Employees;

public record EndWorkShiftCommand(long EmployeeId) : IRequest<WorkShiftDto>;

public class EndWorkShiftCommandHandler : IRequestHandler<EndWorkShiftCommand, WorkShiftDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<EndWorkShiftCommandHandler> _logger;

    public EndWorkShiftCommandHandler(IApplicationDbContext context, ILogger<EndWorkShiftCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<WorkShiftDto> Handle(EndWorkShiftCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Завершение смены для сотрудника ID: {EmployeeId}", request.EmployeeId);

        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == request.EmployeeId && e.IsActive, cancellationToken);

        if (employee == null)
        {
            throw new Exception("Активный сотрудник не найден");
        }

        var openShift = await _context.WorkShifts
            .FirstOrDefaultAsync(ws => ws.EmployeeId == request.EmployeeId && ws.EndTime == null, cancellationToken);

        if (openShift == null)
        {
            throw new Exception("Нет открытой смены для завершения.");
        }

        openShift.EndTime = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Смена завершена для сотрудника ID: {EmployeeId}, ShiftId: {ShiftId}", request.EmployeeId, openShift.Id);

        return new WorkShiftDto
        {
            Id = openShift.Id,
            EmployeeId = openShift.EmployeeId,
            ShiftDate = openShift.ShiftDate,
            StartTime = openShift.StartTime,
            EndTime = openShift.EndTime
        };
    }
}