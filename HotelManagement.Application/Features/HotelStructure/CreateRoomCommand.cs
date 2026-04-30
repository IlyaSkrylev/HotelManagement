using HotelManagement.Application.Abstractions;
using HotelManagement.Application.DTOs;
using HotelManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.HotelStructure;

public record CreateRoomCommand(
    long HotelId,
    long FloorId,
    string RoomNumber,
    long RoomStatusId,
    string? Description) : IRequest<RoomDto>;

public class CreateRoomCommandHandler : IRequestHandler<CreateRoomCommand, RoomDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CreateRoomCommandHandler> _logger;

    public CreateRoomCommandHandler(IApplicationDbContext context, ILogger<CreateRoomCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<RoomDto> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Создание номера для отеля ID: {HotelId}", request.HotelId);

        var existingRoom = await _context.HotelRooms
            .FirstOrDefaultAsync(r => r.HotelId == request.HotelId && r.RoomNumber == request.RoomNumber, cancellationToken);

        if (existingRoom != null)
        {
            throw new Exception($"Номер {request.RoomNumber} уже существует");
        }

        var room = new HotelRoom
        {
            HotelId = request.HotelId,
            FloorId = request.FloorId,
            RoomNumber = request.RoomNumber,
            RoomStatusId = request.RoomStatusId,
            Description = request.Description
        };

        _context.HotelRooms.Add(room);
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