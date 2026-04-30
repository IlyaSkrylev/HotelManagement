namespace HotelManagement.Application.DTOs;

public class ApprovedResumeUserDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Patronymic { get; set; }
    public string? AvatarUrl { get; set; }
    public string DesiredPosition { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}