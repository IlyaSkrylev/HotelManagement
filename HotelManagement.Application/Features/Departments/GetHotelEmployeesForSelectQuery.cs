using HotelManagement.Application.Abstractions;
using HotelManagement.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.Departments;

public record GetHotelEmployeesForSelectQuery(long HotelId, string? SearchTerm = null) : IRequest<List<EmployeeForSelectDto>>;

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
        _logger.LogInformation("Запрос сотрудников для выпадающего списка, HotelId: {HotelId}", request.HotelId);

        var query = _context.Employees
            .Include(e => e.User)
            .Where(e => e.HotelId == request.HotelId && e.IsActive);

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
                AvatarUrl = e.User.AvatarUrl
            })
            .ToListAsync(cancellationToken);

        return employees;
    }
}