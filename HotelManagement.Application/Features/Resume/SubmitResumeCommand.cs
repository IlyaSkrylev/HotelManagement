using HotelManagement.Application.Abstractions;
using HotelManagement.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Application.Features.Resume;

public record SubmitResumeCommand(
    long HotelId,
    string DesiredPosition,
    string? Experience,
    string? Education,
    bool UseProfileResume,
    IFormFile? ResumeFile
) : IRequest<SubmitResumeResponse>;

public record SubmitResumeResponse(long Id, string Status);

public class SubmitResumeCommandHandler : IRequestHandler<SubmitResumeCommand, SubmitResumeResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public SubmitResumeCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<SubmitResumeResponse> Handle(SubmitResumeCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == _currentUserService.UserId, cancellationToken);

        if (user == null)
            throw new UnauthorizedAccessException("Пользователь не найден");

        var hotel = await _context.Hotels
            .FirstOrDefaultAsync(h => h.Id == request.HotelId, cancellationToken);

        if (hotel == null)
            throw new ArgumentException("Гостиница не найдена");

        string? fileUrl = null;
        if (request.UseProfileResume)
        {
            fileUrl = user?.ResumeUrl;
        }
        else if (request.ResumeFile != null && request.ResumeFile.Length > 0)
        {
            fileUrl = await _fileStorageService.SaveFileAsync(request.ResumeFile, "users/resumes", cancellationToken);
        }

        var resume = new Domain.Entities.Resume
        {
            UserId = user.Id,
            HotelId = request.HotelId,
            DesiredPosition = request.DesiredPosition,
            Experience = request.Experience,
            Education = request.Education,
            FileUrl = fileUrl,
            StatusId = 1,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _context.Resumes.Add(resume);
        await _context.SaveChangesAsync(cancellationToken);

        return new SubmitResumeResponse(resume.Id, "pending");
    }
}