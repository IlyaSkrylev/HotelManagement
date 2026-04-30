using HotelManagement.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.Resumes;

public record UpdateResumeStatusCommand(
    long ResumeId,
    long StatusId
) : IRequest<UpdateResumeStatusResponse>;

public record UpdateResumeStatusResponse(bool Success, string Message);

public class UpdateResumeStatusCommandHandler : IRequestHandler<UpdateResumeStatusCommand, UpdateResumeStatusResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<UpdateResumeStatusCommandHandler> _logger;
    private readonly ICurrentUserService _currentUserService;

    public UpdateResumeStatusCommandHandler(
        IApplicationDbContext context,
        ILogger<UpdateResumeStatusCommandHandler> logger,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<UpdateResumeStatusResponse> Handle(UpdateResumeStatusCommand request, CancellationToken cancellationToken)
    {
        var resume = await _context.Resumes
            .FirstOrDefaultAsync(r => r.Id == request.ResumeId, cancellationToken);

        if (resume == null)
        {
            return new UpdateResumeStatusResponse(false, "Резюме не найдено");
        }

        var oldStatusId = resume.StatusId;

        resume.StatusId = request.StatusId;
        resume.ReviewedAt = DateTimeOffset.UtcNow;
        resume.ReviewedById = _currentUserService.UserId;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Статус резюме ID: {ResumeId} изменён с {OldStatusId} на {NewStatusId}",
            resume.Id, oldStatusId, request.StatusId);

        return new UpdateResumeStatusResponse(true, "Статус успешно обновлён");
    }
}