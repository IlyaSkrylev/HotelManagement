// GetRoomsQuery.cs
using HotelManagement.Application.Abstractions;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.HotelStructure;

public record GetRoomsQuery(long HotelId, long? FloorId = null, int Page = 1, int PageSize = 20) : IRequest<PaginatedResult<RoomDto>>;

public class GetRoomsQueryHandler : IRequestHandler<GetRoomsQuery, PaginatedResult<RoomDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetRoomsQueryHandler> _logger;

    public GetRoomsQueryHandler(IApplicationDbContext context, ILogger<GetRoomsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PaginatedResult<RoomDto>> Handle(GetRoomsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Запрос номеров отеля ID: {HotelId}", request.HotelId);

        var query = _context.HotelRooms
            .Include(r => r.Floor)
            .Include(r => r.RoomStatus)
            .Where(r => r.HotelId == request.HotelId);

        if (request.FloorId.HasValue)
        {
            query = query.Where(r => r.FloorId == request.FloorId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(r => r.Floor.FloorNumber)
            .ThenBy(r => r.RoomNumber)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(r => new RoomDto
            {
                Id = r.Id,
                HotelId = r.HotelId,
                FloorId = r.FloorId,
                FloorName = r.Floor.Name,
                FloorNumber = r.Floor.FloorNumber,
                RoomNumber = r.RoomNumber,
                RoomStatusId = r.RoomStatusId,
                RoomStatusName = r.RoomStatus.Name,
                RoomStatusColor = r.RoomStatus.Color,
                Description = r.Description,
            })
            .ToListAsync(cancellationToken);

        return new PaginatedResult<RoomDto>(items, totalCount, request.Page, request.PageSize);
    }
}