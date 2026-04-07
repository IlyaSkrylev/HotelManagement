using HotelManagement.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.Auth;

public record RefreshTokenCommand(string RefreshToken) : IRequest<RefreshTokenResponse>;

public record RefreshTokenResponse(string AccessToken, string RefreshToken);

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RefreshTokenCommandHandler(
        IApplicationDbContext context,
        ILogger<RefreshTokenCommandHandler> logger,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _context = context;
        _logger = logger;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken, cancellationToken);

        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Недействительный или истёкший refresh token");
        }

        var newAccessToken = _jwtTokenGenerator.GenerateAccessToken(user.Id, user.Email, user.FirstName, user.LastName);
        var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync(cancellationToken);

        return new RefreshTokenResponse(newAccessToken, newRefreshToken);
    }
}