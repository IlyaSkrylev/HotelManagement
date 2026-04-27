using HotelManagement.Application.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Application.Features.Profile;

public record UploadResumeCommand(
    IFormFile Resume
) : IRequest<string>;

public class UploadResumeCommandHandler : IRequestHandler<UploadResumeCommand, string>
{
    private readonly IApplicationDbContext _context;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICurrentUserService _currentUserService;

    public UploadResumeCommandHandler(
        IApplicationDbContext context,
        IFileStorageService fileStorageService,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _fileStorageService = fileStorageService;
        _currentUserService = currentUserService;
    }

    public async Task<string> Handle(UploadResumeCommand request, CancellationToken cancellationToken)
    {
        if (request.Resume == null || request.Resume.Length == 0)
            throw new ArgumentException("Файл не выбран");

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == _currentUserService.UserId, cancellationToken);

        if (user == null)
            throw new UnauthorizedAccessException("Пользователь не найден");

        if (!string.IsNullOrEmpty(user.ResumeUrl))
        {
            _fileStorageService.DeleteFile(user.ResumeUrl);
        }

        var fileUrl = await _fileStorageService.SaveFileAsync(request.Resume, "users/resumes", cancellationToken);
        user.ResumeUrl = fileUrl;
        await _context.SaveChangesAsync(cancellationToken);

        return fileUrl;
    }
}