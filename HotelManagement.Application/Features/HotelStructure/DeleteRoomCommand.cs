// DeleteRoomCommand.cs
using HotelManagement.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.HotelStructure;

public record DeleteRoomCommand(long Id) : IRequest<bool>;

public class DeleteRoomCommandHandler : IRequestHandler<DeleteRoomCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<DeleteRoomCommandHandler> _logger;

    public DeleteRoomCommandHandler(IApplicationDbContext context, ILogger<DeleteRoomCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteRoomCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Удаление номера ID: {Id}", request.Id);

        var room = await _context.HotelRooms
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        if (room == null)
        {
            return false;
        }

        _context.HotelRooms.Remove(room);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}