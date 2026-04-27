using HotelManagement.Application.Abstractions;
using HotelManagement.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Application.Features.Profile;

public record UpdateProfileCommand(
    string FirstName,
    string LastName,
    string? Patronymic,
    string? Phone,
    DateTime? BirthDate,
    IFormFile? Avatar
) : IRequest<UserProfileDto>;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, UserProfileDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public UpdateProfileCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<UserProfileDto> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"=== UpdateProfile Start ===");
            Console.WriteLine($"FirstName: {request.FirstName}");
            Console.WriteLine($"LastName: {request.LastName}");
            Console.WriteLine($"BirthDate: {request.BirthDate?.ToString("yyyy-MM-dd") ?? "null"}");
            Console.WriteLine($"Avatar: {(request.Avatar != null ? request.Avatar.FileName : "null")}");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == _currentUserService.UserId, cancellationToken);

            if (user == null)
                throw new UnauthorizedAccessException("Пользователь не найден");

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Patronymic = request.Patronymic;
            user.Phone = request.Phone;

            // Для PostgreSQL timestamptz значение должно быть UTC.
            user.BirthDate = request.BirthDate.HasValue
                ? DateTime.SpecifyKind(request.BirthDate.Value, DateTimeKind.Utc)
                : null;

            user.UpdatedAt = DateTimeOffset.UtcNow;

            if (request.Avatar != null && request.Avatar.Length > 0)
            {
                if (!string.IsNullOrEmpty(user.AvatarUrl))
                {
                    _fileStorageService.DeleteFile(user.AvatarUrl);
                }
                user.AvatarUrl = await _fileStorageService.SaveFileAsync(request.Avatar, "users/avatars", cancellationToken);
                Console.WriteLine($"Avatar saved to: {user.AvatarUrl}");
            }

            Console.WriteLine($"Saving to database...");
            await _context.SaveChangesAsync(cancellationToken);
            Console.WriteLine($"Save successful!");

            return new UserProfileDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Patronymic = user.Patronymic,
                Phone = user.Phone,
                BirthDate = user.BirthDate,
                AvatarUrl = user.AvatarUrl,
                ResumeUrl = user.ResumeUrl
            };
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine($"DbUpdateException: {ex.Message}");
            Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            throw;
        }
    }
}