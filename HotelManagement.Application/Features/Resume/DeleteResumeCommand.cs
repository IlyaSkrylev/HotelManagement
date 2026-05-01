using HotelManagement.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.Resumes;

public record DeleteResumeCommand(long ResumeId) : IRequest<bool>;

public class DeleteResumeCommandHandler : IRequestHandler<DeleteResumeCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<DeleteResumeCommandHandler> _logger;
    private readonly ICurrentUserService _currentUserService;

    public DeleteResumeCommandHandler(
        IApplicationDbContext context,
        ILogger<DeleteResumeCommandHandler> logger,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(DeleteResumeCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Удаление резюме ID: {ResumeId} пользователем {UserId}",
            request.ResumeId, _currentUserService.UserId);

        var resume = await _context.Resumes
            .FirstOrDefaultAsync(r => r.Id == request.ResumeId, cancellationToken);

        if (resume == null)
        {
            _logger.LogWarning("Резюме с ID {ResumeId} не найдено", request.ResumeId);
            return false;
        }

        _context.Resumes.Remove(resume);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Резюме ID {ResumeId} успешно удалено", request.ResumeId);

        return true;
    }
}