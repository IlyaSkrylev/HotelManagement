using HotelManagement.Application.Common;
using HotelManagement.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using HotelManagement.Application.DTOs;

namespace HotelManagement.Application.Features.Hotels
{
    public record GetHotelsQuery(int Page = 1, int PageSize = 10) : IRequest<PaginatedResult<HotelDto>>;

    public class GetHotelsQueryHandler : IRequestHandler<GetHotelsQuery, PaginatedResult<HotelDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetHotelsQueryHandler> _logger;

        public GetHotelsQueryHandler(IApplicationDbContext context, ILogger<GetHotelsQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PaginatedResult<HotelDto>> Handle(GetHotelsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Запрос списка гостиниц. Страница: {Page}, Размер страницы: {PageSize}", request.Page, request.PageSize);

            var query = _context.Hotels.AsNoTracking();

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

            _logger.LogInformation("Найдено {Count} гостиниц из {TotalCount}", items.Count, totalCount);

            return new PaginatedResult<HotelDto>(items, totalCount, request.Page, request.PageSize);
        }
    }
}
