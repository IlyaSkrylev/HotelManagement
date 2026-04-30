using HotelManagement.Application.Abstractions;
using HotelManagement.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.HotelStructure;

public record GetRoomStatusesQuery : IRequest<List<RoomStatusDto>>;

public class GetRoomStatusesQueryHandler : IRequestHandler<GetRoomStatusesQuery, List<RoomStatusDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetRoomStatusesQueryHandler> _logger;

    public GetRoomStatusesQueryHandler(IApplicationDbContext context, ILogger<GetRoomStatusesQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<RoomStatusDto>> Handle(GetRoomStatusesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Запрос статусов номеров");

        var statuses = await _context.RoomStatuses
            .OrderBy(s => s.Id)
            .Select(s => new RoomStatusDto
            {
                Id = s.Id,
                Code = s.Code,
                Name = s.Name,
                Color = s.Color,
                Description = s.Description
            })
            .ToListAsync(cancellationToken);

        return statuses;
    }
}