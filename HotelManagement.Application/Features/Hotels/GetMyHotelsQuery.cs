using HotelManagement.Application.Abstractions;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.Hotels;

public record GetMyHotelsQuery(int Page = 1, int PageSize = 10) : IRequest<PaginatedResult<MyHotelDto>>;

public class GetMyHotelsQueryHandler : IRequestHandler<GetMyHotelsQuery, PaginatedResult<MyHotelDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetMyHotelsQueryHandler> _logger;
    private readonly ICurrentUserService _currentUserService;

    public GetMyHotelsQueryHandler(
        IApplicationDbContext context,
        ILogger<GetMyHotelsQueryHandler> logger,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<PaginatedResult<MyHotelDto>> Handle(GetMyHotelsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        _logger.LogInformation("Запрос списка моих гостиниц. UserId: {UserId}, Страница: {Page}, Размер страницы: {PageSize}",
            userId, request.Page, request.PageSize);

        var query = _context.UserHotelRoles
            .Include(uhr => uhr.Hotel)
            .Include(uhr => uhr.Role)
            .Where(uhr => uhr.UserId == userId && uhr.Hotel != null)
            .Where(uhr => _context.Employees
                .Any(e => e.UserId == userId && e.HotelId == uhr.HotelId && e.IsActive))
            .Select(uhr => new MyHotelDto
            {
                Id = uhr.Hotel.Id,
                Name = uhr.Hotel.Name,
                Address = uhr.Hotel.Address,
                Phone = uhr.Hotel.Phone,
                Email = uhr.Hotel.Email,
                Description = uhr.Hotel.Description,
                ImageUrl = uhr.Hotel.ImageUrl,
                RoleId = uhr.RoleId,
                RoleCode = uhr.Role.Code,
                RoleName = uhr.Role.Name,
                AssignedAt = uhr.AssignedAt,
                Position = _context.Employees
                    .Where(e => e.UserId == userId && e.HotelId == uhr.HotelId && e.IsActive)
                    .Select(e => e.Position)
                    .FirstOrDefault() ?? string.Empty
            })
            .Distinct()
            .AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(h => h.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Найдено {Count} гостиниц через UserHotelRole из {TotalCount}", items.Count, totalCount);

        return new PaginatedResult<MyHotelDto>(items, totalCount, request.Page, request.PageSize);
    }
}