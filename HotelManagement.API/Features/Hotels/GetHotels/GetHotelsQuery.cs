using HotelManagement.API.Common;
using HotelManagement.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.API.Features.Hotels.GetHotels
{
    public record GetHotelsQuery(int Page = 1, int PageSize = 10) : IRequest<PaginatedResult<HotelDto>>;

    public class HotelDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

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
                    Email = h.Email
                })
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Найдено {Count} гостиниц из {TotalCount}", items.Count, totalCount);

            return new PaginatedResult<HotelDto>(items, totalCount, request.Page, request.PageSize);
        }
    }
}
