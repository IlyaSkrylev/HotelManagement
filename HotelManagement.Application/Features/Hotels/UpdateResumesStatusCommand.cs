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
        _logger.LogInformation("Обновление статуса резюме ID: {ResumeId} на StatusId: {StatusId}",
            request.ResumeId, request.StatusId);

        var resume = await _context.Resumes
            .FirstOrDefaultAsync(r => r.Id == request.ResumeId, cancellationToken);

        if (resume == null)
        {
            return new UpdateResumeStatusResponse(false, "Резюме не найдено");
        }

        // Получаем Employee ID текущего пользователя в этом отеле
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.UserId == _currentUserService.UserId && e.HotelId == resume.HotelId, cancellationToken);

        if (employee == null)
        {
            _logger.LogWarning("Сотрудник не найден для UserId: {UserId} в отеле {HotelId}",
                _currentUserService.UserId, resume.HotelId);

            // Если сотрудник не найден, обновляем только статус, без reviewed_by_id
            resume.StatusId = request.StatusId;
            resume.ReviewedAt = DateTimeOffset.UtcNow;
            // reviewed_by_id оставляем null или существующим
        }
        else
        {
            resume.StatusId = request.StatusId;
            resume.ReviewedAt = DateTimeOffset.UtcNow;
            resume.ReviewedById = employee.Id;
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Статус резюме ID: {ResumeId} изменён на {NewStatusId}, reviewed_by: {ReviewedBy}",
            resume.Id, request.StatusId, employee?.Id);

        return new UpdateResumeStatusResponse(true, "Статус успешно обновлён");
    }
}