using HotelManagement.Application.Abstractions;
using HotelManagement.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.Employees;

public record GetCurrentUserEmployeeInfoQuery(long HotelId) : IRequest<CurrentUserEmployeeInfoDto?>;

public class GetCurrentUserEmployeeInfoQueryHandler : IRequestHandler<GetCurrentUserEmployeeInfoQuery, CurrentUserEmployeeInfoDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetCurrentUserEmployeeInfoQueryHandler> _logger;
    private readonly ICurrentUserService _currentUserService;

    public GetCurrentUserEmployeeInfoQueryHandler(
        IApplicationDbContext context,
        ILogger<GetCurrentUserEmployeeInfoQueryHandler> logger,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<CurrentUserEmployeeInfoDto?> Handle(GetCurrentUserEmployeeInfoQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Запрос информации о сотруднике для текущего пользователя в отеле ID: {HotelId}", request.HotelId);

        var employee = await _context.Employees
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.UserId == _currentUserService.UserId && e.HotelId == request.HotelId, cancellationToken);

        if (employee == null)
        {
            _logger.LogWarning("Сотрудник не найден для пользователя {UserId} в отеле {HotelId}",
                _currentUserService.UserId, request.HotelId);
            return null;
        }

        return new CurrentUserEmployeeInfoDto
        {
            Id = employee.Id,
            DepartmentId = employee.DepartmentId,
            DepartmentName = employee.Department?.Name,
            Position = employee.Position,
            HireDate = employee.HireDate,
            IsActive = employee.IsActive
        };
    }
}