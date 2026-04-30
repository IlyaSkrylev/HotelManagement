using HotelManagement.Application.Abstractions;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.Departments;

public record GetDepartmentsQuery(
    long HotelId,
    string? SearchTerm = null,
    int Page = 1,
    int PageSize = 20) : IRequest<PaginatedResult<DepartmentDto>>;

public class GetDepartmentsQueryHandler : IRequestHandler<GetDepartmentsQuery, PaginatedResult<DepartmentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetDepartmentsQueryHandler> _logger;

    public GetDepartmentsQueryHandler(IApplicationDbContext context, ILogger<GetDepartmentsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PaginatedResult<DepartmentDto>> Handle(GetDepartmentsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Запрос отделов отеля ID: {HotelId}, SearchTerm: {SearchTerm}", request.HotelId, request.SearchTerm);

        var query = _context.Departments
            .Include(d => d.Manager)
            .ThenInclude(m => m.User)
            .Where(d => d.HotelId == request.HotelId);

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.Trim().ToLower();
            query = query.Where(d =>
                d.Name.ToLower().Contains(searchTerm) ||
                (d.Manager != null && d.Manager.User != null &&
                 (d.Manager.User.FirstName.ToLower().Contains(searchTerm) ||
                  d.Manager.User.LastName.ToLower().Contains(searchTerm))));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(d => d.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description,
                ManagerId = d.ManagerId,
                ManagerName = d.Manager != null && d.Manager.User != null
                    ? d.Manager.User.LastName + " " + d.Manager.User.FirstName
                    : null,
                ManagerPosition = d.Manager != null ? d.Manager.Position : null,
                EmployeeCount = _context.Employees.Count(e => e.DepartmentId == d.Id && e.IsActive),
                HotelId = d.HotelId,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return new PaginatedResult<DepartmentDto>(items, totalCount, request.Page, request.PageSize);
    }
}