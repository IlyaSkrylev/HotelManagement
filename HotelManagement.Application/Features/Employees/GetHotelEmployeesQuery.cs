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
    bool IncludeInactive = false,
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
            .Include(e => e.ShiftType)
            .Where(e => e.HotelId == request.HotelId);

        if (request.IncludeInactive)
        {
            query = query.Where(e => !e.IsActive);
        }
        else
        {
            query = query.Where(e => e.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.Trim().ToLower();
            query = query.Where(e =>
                e.User.FirstName.ToLower().Contains(searchTerm) ||
                e.User.LastName.ToLower().Contains(searchTerm) ||
                (e.User.Patronymic != null && e.User.Patronymic.ToLower().Contains(searchTerm))
            );
        }

        if (!string.IsNullOrWhiteSpace(request.DepartmentName) && request.DepartmentName != "all")
        {
            query = query.Where(e => e.Department.Name == request.DepartmentName);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(e => e.User.LastName)
            .ThenBy(e => e.User.FirstName)
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
                HireDate = e.HireDate,
                DismissalDate = e.DismissalDate,          
                DismissalReason = e.DismissalReason,
                RoleId = _context.UserHotelRoles
                    .Where(uhr => uhr.UserId == e.UserId && uhr.HotelId == request.HotelId)
                    .Select(uhr => uhr.RoleId)
                    .FirstOrDefault(),
                RoleCode = _context.UserHotelRoles
                    .Where(uhr => uhr.UserId == e.UserId && uhr.HotelId == request.HotelId)
                    .Select(uhr => uhr.Role.Code)
                    .FirstOrDefault() ?? string.Empty,
                Salary = e.Salary,
                SalarySupplement = e.SalarySupplement,
                WorkingDayShifts = e.ShiftType != null ? e.ShiftType.WorkingDayShifts : 0,
                WorkingNightShifts = e.ShiftType != null ? e.ShiftType.WorkingNightShifts : 0,
                RestDays = e.ShiftType != null ? e.ShiftType.RestDays : 0,
                DayShiftStart = e.ShiftType != null ? e.ShiftType.DayShiftStartTime.ToString(@"hh\:mm") : "09:00",
                DayShiftEnd = e.ShiftType != null ? e.ShiftType.DayShiftEndTime.ToString(@"hh\:mm") : "18:00",
                NightShiftStart = e.ShiftType != null ? e.ShiftType.NightShiftStartTime.ToString(@"hh\:mm") : "21:00",
                NightShiftEnd = e.ShiftType != null ? e.ShiftType.NightShiftEndTime.ToString(@"hh\:mm") : "06:00",
                ShiftCycleStartsWithDay = e.ShiftCycleStartsWithDay,
                ShiftCycleStartDate = e.ShiftCycleStartDate,
                TotalCycleDays = e.ShiftType != null ? e.ShiftType.TotalCycleDays : 0
            })
            .ToListAsync(cancellationToken);

        return new PaginatedResult<EmployeeListItemDto>(items, totalCount, request.Page, request.PageSize);
    }
}