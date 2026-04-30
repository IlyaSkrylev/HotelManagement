using HotelManagement.Application.Abstractions;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.Employees;

public record GetHotelEmployeesQuery(
    long HotelId,
    string? SearchTerm = null,
    string? DepartmentName = null,
    int Page = 1,
    int PageSize = 20) : IRequest<PaginatedResult<EmployeeListItemDto>>;

public class GetHotelEmployeesQueryHandler : IRequestHandler<GetHotelEmployeesQuery, PaginatedResult<EmployeeListItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetHotelEmployeesQueryHandler> _logger;

    public GetHotelEmployeesQueryHandler(IApplicationDbContext context, ILogger<GetHotelEmployeesQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PaginatedResult<EmployeeListItemDto>> Handle(GetHotelEmployeesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Запрос сотрудников отеля ID: {HotelId}, SearchTerm: {SearchTerm}, DepartmentName: {DepartmentName}, Page: {Page}",
            request.HotelId, request.SearchTerm, request.DepartmentName, request.Page);

        var query = _context.Employees
            .Include(e => e.User)
            .Include(e => e.Department)
            .Where(e => e.HotelId == request.HotelId && e.IsActive);

        // Поиск по ФИО
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.Trim().ToLower();
            query = query.Where(e =>
                e.User.FirstName.ToLower().Contains(searchTerm) ||
                e.User.LastName.ToLower().Contains(searchTerm) ||
                (e.User.Patronymic != null && e.User.Patronymic.ToLower().Contains(searchTerm))
            );
        }

        // Фильтрация по отделу
        if (!string.IsNullOrWhiteSpace(request.DepartmentName) && request.DepartmentName != "all")
        {
            query = query.Where(e => e.Department.Name == request.DepartmentName);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(e => e.User.LastName)      // ✅ Через e.User
            .ThenBy(e => e.User.FirstName)      // ✅ Через e.User
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(e => new EmployeeListItemDto
            {
                Id = e.Id,
                UserId = e.UserId,
                FirstName = e.User.FirstName,
                LastName = e.User.LastName,
                Patronymic = e.User.Patronymic,
                AvatarUrl = e.User.AvatarUrl,
                Position = e.Position,
                DepartmentId = e.DepartmentId,
                DepartmentName = e.Department != null ? e.Department.Name : string.Empty,
                HireDate = e.HireDate
            })
            .ToListAsync(cancellationToken);

        return new PaginatedResult<EmployeeListItemDto>(items, totalCount, request.Page, request.PageSize);
    }
}