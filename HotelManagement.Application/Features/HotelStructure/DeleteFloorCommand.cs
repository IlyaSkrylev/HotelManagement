using HotelManagement.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.HotelStructure;

public record DeleteFloorCommand(long Id) : IRequest<bool>;

public class DeleteFloorCommandHandler : IRequestHandler<DeleteFloorCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<DeleteFloorCommandHandler> _logger;

    public DeleteFloorCommandHandler(IApplicationDbContext context, ILogger<DeleteFloorCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteFloorCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Удаление этажа ID: {Id}", request.Id);

        var floor = await _context.HotelFloors
            .Include(f => f.HotelRooms)
            .FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken);

        if (floor == null)
        {
            return false;
        }

        if (floor.HotelRooms.Any())
        {
            throw new Exception("Нельзя удалить этаж, на котором есть номера");
        }

        _context.HotelFloors.Remove(floor);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}