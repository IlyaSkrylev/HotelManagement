using HotelManagement.Application.Abstractions;
using HotelManagement.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.HotelStructure;

public record UpdateRoomCommand(
    long Id,
    long FloorId,
    string RoomNumber,
    long RoomStatusId,
    string? Description) : IRequest<RoomDto>;

public class UpdateRoomCommandHandler : IRequestHandler<UpdateRoomCommand, RoomDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<UpdateRoomCommandHandler> _logger;

    public UpdateRoomCommandHandler(IApplicationDbContext context, ILogger<UpdateRoomCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<RoomDto> Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Обновление номера ID: {Id}", request.Id);

        var room = await _context.HotelRooms
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (room == null)
        {
            throw new Exception("Номер не найден");
        }

        room.FloorId = request.FloorId;
        room.RoomNumber = request.RoomNumber;
        room.RoomStatusId = request.RoomStatusId;
        room.Description = request.Description;

        await _context.SaveChangesAsync(cancellationToken);

        var floor = await _context.HotelFloors
            .FirstOrDefaultAsync(f => f.Id == request.FloorId, cancellationToken);

        var roomStatus = await _context.RoomStatuses
            .FirstOrDefaultAsync(rs => rs.Id == request.RoomStatusId, cancellationToken);

        return new RoomDto
        {
            Id = room.Id,
            HotelId = room.HotelId,
            FloorId = room.FloorId,
            FloorName = floor?.Name ?? string.Empty,
            FloorNumber = floor?.FloorNumber ?? 0,
            RoomNumber = room.RoomNumber,
            RoomStatusId = room.RoomStatusId,
            RoomStatusName = roomStatus?.Name ?? string.Empty,
            RoomStatusColor = roomStatus?.Color,
            Description = room.Description
        };
    }
}