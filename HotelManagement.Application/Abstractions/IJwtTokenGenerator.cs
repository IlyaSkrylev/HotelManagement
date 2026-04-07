namespace HotelManagement.Application.Abstractions;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(long userId, string email, string firstName, string lastName);
    string GenerateRefreshToken();
}