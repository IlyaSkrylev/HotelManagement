namespace HotelManagement.Application.Abstractions;

public interface ICurrentUserService
{
    long UserId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
}