using HotelManagement.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.Auth;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<LoginResponse>;

public record LoginResponse(
    long Id,
    string Email,
    string FirstName,
    string LastName,
    string AccessToken,
    string RefreshToken
);

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<LoginCommandHandler> _logger;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(
        IApplicationDbContext context,
        ILogger<LoginCommandHandler> logger,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _context = context;
        _logger = logger;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Попытка входа: {Email}", request.Email);

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user == null)
        {
            throw new UnauthorizedAccessException("Неверный email или пароль");
        }

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Неверный email или пароль");
        }

        user.LastLogin = DateTimeOffset.UtcNow;

        var accessToken = _jwtTokenGenerator.GenerateAccessToken(user.Id, user.Email, user.FirstName, user.LastName);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtTokenGenerator.GetRefreshTokenExpiryDays());

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Пользователь вошёл: {Email}", request.Email);

        return new LoginResponse(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            accessToken,
            refreshToken
        );
    }
}