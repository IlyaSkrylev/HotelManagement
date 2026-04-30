using HotelManagement.Application.Abstractions;
using HotelManagement.Application.DTOs;
using HotelManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.HotelStructure;

public record CreateFloorCommand(long HotelId, int FloorNumber, string Name, string? Description) : IRequest<FloorDto>;

public class CreateFloorCommandHandler : IRequestHandler<CreateFloorCommand, FloorDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CreateFloorCommandHandler> _logger;

    public CreateFloorCommandHandler(IApplicationDbContext context, ILogger<CreateFloorCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<FloorDto> Handle(CreateFloorCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Создание этажа для отеля ID: {HotelId}", request.HotelId);

        var existingFloor = await _context.HotelFloors
            .FirstOrDefaultAsync(f => f.HotelId == request.HotelId && f.FloorNumber == request.FloorNumber, cancellationToken);

        if (existingFloor != null)
        {
            throw new Exception($"Этаж {request.FloorNumber} уже существует");
        }

        var floor = new HotelFloor
        {
            HotelId = request.HotelId,
            FloorNumber = request.FloorNumber,
            Name = request.Name,
            Description = request.Description
        };

        _context.HotelFloors.Add(floor);
        await _context.SaveChangesAsync(cancellationToken);

        return new FloorDto
        {
            Id = floor.Id,
            HotelId = floor.HotelId,
            FloorNumber = floor.FloorNumber,
            Name = floor.Name,
            Description = floor.Description,
            RoomsCount = 0
        };
    }
}