using HotelManagement.Application.Abstractions;
using HotelManagement.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.HotelStructure;

public record UpdateFloorCommand(long Id, int FloorNumber, string Name, string? Description) : IRequest<FloorDto>;

public class UpdateFloorCommandHandler : IRequestHandler<UpdateFloorCommand, FloorDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<UpdateFloorCommandHandler> _logger;

    public UpdateFloorCommandHandler(IApplicationDbContext context, ILogger<UpdateFloorCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<FloorDto> Handle(UpdateFloorCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Обновление этажа ID: {Id}", request.Id);

        var floor = await _context.HotelFloors
            .FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken);

        if (floor == null)
        {
            throw new Exception("Этаж не найден");
        }

        floor.FloorNumber = request.FloorNumber;
        floor.Name = request.Name;
        floor.Description = request.Description;

        await _context.SaveChangesAsync(cancellationToken);

        return new FloorDto
        {
            Id = floor.Id,
            HotelId = floor.HotelId,
            FloorNumber = floor.FloorNumber,
            Name = floor.Name,
            Description = floor.Description,
            RoomsCount = _context.HotelRooms.Count(r => r.FloorId == floor.Id)
        };
    }
}