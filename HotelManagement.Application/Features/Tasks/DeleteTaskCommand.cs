using HotelManagement.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.Tasks;

public record DeleteTaskCommand(long Id) : IRequest<bool>;

public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<DeleteTaskCommandHandler> _logger;

    public DeleteTaskCommandHandler(IApplicationDbContext context, ILogger<DeleteTaskCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Удаление задачи ID: {Id}", request.Id);

        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (task == null)
        {
            return false;
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}