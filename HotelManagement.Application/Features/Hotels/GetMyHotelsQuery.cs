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

        var query = _context.Employees
            .Where(e => e.UserId == userId && e.IsActive)
            .Select(e => new MyHotelDto
            {
                Id = e.Hotel.Id,
                Name = e.Hotel.Name,
                Address = e.Hotel.Address,
                Phone = e.Hotel.Phone,
                Email = e.Hotel.Email,
                Description = e.Hotel.Description,
                ImageUrl = e.Hotel.ImageUrl,
                Position = e.Position,
                DepartmentId = e.DepartmentId,
                DepartmentName = e.Department.Name,
                HireDate = e.HireDate
            })
            .AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Найдено {Count} моих гостиниц из {TotalCount}", items.Count, totalCount);

        return new PaginatedResult<MyHotelDto>(items, totalCount, request.Page, request.PageSize);
    }
}