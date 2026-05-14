using HotelManagement.Application.Abstractions;
using HotelManagement.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.Departments;

public record GetHotelEmployeesForSelectQuery(
    long HotelId,
    string? SearchTerm = null,
    string? RoleCode = null) : IRequest<List<EmployeeForSelectDto>>;

public class GetHotelEmployeesForSelectQueryHandler : IRequestHandler<GetHotelEmployeesForSelectQuery, List<EmployeeForSelectDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetHotelEmployeesForSelectQueryHandler> _logger;

    public GetHotelEmployeesForSelectQueryHandler(IApplicationDbContext context, ILogger<GetHotelEmployeesForSelectQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<EmployeeForSelectDto>> Handle(GetHotelEmployeesForSelectQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Запрос сотрудников для выпадающего списка, HotelId: {HotelId}, RoleCode: {RoleCode}",
            request.HotelId, request.RoleCode);

        var query = _context.Employees
            .Include(e => e.User)
            .Where(e => e.HotelId == request.HotelId && e.IsActive);

        if (!string.IsNullOrWhiteSpace(request.RoleCode))
        {
            var roleCodes = request.RoleCode == "manager"
                ? new[] { "manager", "admin" }
                : new[] { request.RoleCode };

            query = query.Where(e => _context.UserHotelRoles
                .Any(uhr => uhr.UserId == e.UserId &&
                            uhr.HotelId == request.HotelId &&
                            roleCodes.Contains(uhr.Role.Code)));
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.Trim().ToLower();
            query = query.Where(e =>
                e.User.FirstName.ToLower().Contains(searchTerm) ||
                e.User.LastName.ToLower().Contains(searchTerm) ||
                (e.User.Patronymic != null && e.User.Patronymic.ToLower().Contains(searchTerm)));
        }

        var employees = await query
            .OrderBy(e => e.User.LastName)
            .ThenBy(e => e.User.FirstName)
            .Take(50)
            .Select(e => new EmployeeForSelectDto
            {
                Id = e.Id,
                UserId = e.UserId,
                FullName = e.User.LastName + " " + e.User.FirstName + (e.User.Patronymic != null ? " " + e.User.Patronymic : ""),
                Position = e.Position,
                AvatarUrl = e.User.AvatarUrl,
                RoleCode = _context.UserHotelRoles
                    .Where(uhr => uhr.UserId == e.UserId && uhr.HotelId == request.HotelId)
                    .Select(uhr => uhr.Role.Code)
                    .FirstOrDefault() ?? string.Empty
            })
            .ToListAsync(cancellationToken);

        return employees;
    }
}