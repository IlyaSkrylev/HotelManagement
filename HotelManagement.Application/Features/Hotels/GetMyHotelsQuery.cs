using HotelManagement.Application.Abstractions;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.Hotels;

public record GetMyHotelsQuery(int Page = 1, int PageSize = 10) : IRequest<PaginatedResult<HotelDto>>;

public class GetMyHotelsQueryHandler : IRequestHandler<GetMyHotelsQuery, PaginatedResult<HotelDto>>
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

    public async Task<PaginatedResult<HotelDto>> Handle(GetMyHotelsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        _logger.LogInformation("Запрос списка моих гостиниц. UserId: {UserId}, Страница: {Page}, Размер страницы: {PageSize}",
            userId, request.Page, request.PageSize);

        var query = _context.Hotels
            .Where(h => h.CreatedById == userId)
            .AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(h => new HotelDto
            {
                Id = h.Id,
                Name = h.Name,
                Address = h.Address,
                Phone = h.Phone,
                Email = h.Email,
                Description = h.Description,
                ImageUrl = h.ImageUrl
            })
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Найдено {Count} моих гостиниц из {TotalCount}", items.Count, totalCount);

        return new PaginatedResult<HotelDto>(items, totalCount, request.Page, request.PageSize);
    }
}