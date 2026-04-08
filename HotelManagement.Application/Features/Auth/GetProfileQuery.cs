using HotelManagement.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.Auth;

public record GetProfileQuery : IRequest<GetProfileResponse>;

public record GetProfileResponse(
    long Id,
    string Email,
    string FirstName,
    string LastName,
    string? Patronymic,
    string? Phone,
    DateTime? BirthDate,
    string? AvatarUrl
);

public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, GetProfileResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetProfileQueryHandler> _logger;
    private readonly ICurrentUserService _currentUserService;

    public GetProfileQueryHandler(
        IApplicationDbContext context,
        ILogger<GetProfileQueryHandler> logger,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<GetProfileResponse> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user == null)
        {
            throw new UnauthorizedAccessException("Пользователь не найден");
        }

        return new GetProfileResponse(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.Patronymic,
            user.Phone,
            user.BirthDate,
            user.AvatarUrl
        );
    }
}