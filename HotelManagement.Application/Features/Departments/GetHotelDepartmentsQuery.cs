using HotelManagement.Application.Abstractions;
using HotelManagement.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.Departments;

public record GetHotelDepartmentsQuery(long HotelId) : IRequest<List<DepartmentDto>>;

public class GetHotelDepartmentsQueryHandler : IRequestHandler<GetHotelDepartmentsQuery, List<DepartmentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetHotelDepartmentsQueryHandler> _logger;

    public GetHotelDepartmentsQueryHandler(IApplicationDbContext context, ILogger<GetHotelDepartmentsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<DepartmentDto>> Handle(GetHotelDepartmentsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Запрос отделов отеля ID: {HotelId}", request.HotelId);

        var departments = await _context.Departments
            .Where(d => d.HotelId == request.HotelId)
            .OrderBy(d => d.Name)
            .Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                ManagerId = d.ManagerId,
                ManagerName = d.ManagerId != null
                    ? _context.Users
                        .Where(u => u.Id == d.ManagerId)
                        .Select(u => u.LastName + " " + u.FirstName)
                        .FirstOrDefault()
                    : null,
                EmployeeCount = _context.Employees.Count(e => e.DepartmentId == d.Id && e.IsActive),
                HotelId = d.HotelId,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return departments;
    }
}