using HotelManagement.Application.Abstractions;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.HotelStructure;

public record GetFloorsQuery(long HotelId, int Page = 1, int PageSize = 20) : IRequest<PaginatedResult<FloorDto>>;

public class GetFloorsQueryHandler : IRequestHandler<GetFloorsQuery, PaginatedResult<FloorDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetFloorsQueryHandler> _logger;

    public GetFloorsQueryHandler(IApplicationDbContext context, ILogger<GetFloorsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PaginatedResult<FloorDto>> Handle(GetFloorsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Запрос этажей отеля ID: {HotelId}", request.HotelId);

        var query = _context.HotelFloors
            .Where(f => f.HotelId == request.HotelId)
            .OrderBy(f => f.FloorNumber);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(f => new FloorDto
            {
                Id = f.Id,
                HotelId = f.HotelId,
                FloorNumber = f.FloorNumber,
                Name = f.Name,
                Description = f.Description,
                RoomsCount = _context.HotelRooms.Count(r => r.FloorId == f.Id)
            })
            .ToListAsync(cancellationToken);

        return new PaginatedResult<FloorDto>(items, totalCount, request.Page, request.PageSize);
    }
}