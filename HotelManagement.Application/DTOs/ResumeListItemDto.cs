namespace HotelManagement.Application.DTOs;

public class ResumeListItemDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Patronymic { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? AvatarUrl { get; set; }
    public string DesiredPosition { get; set; } = string.Empty;
    public string? Experience { get; set; }
    public string? Education { get; set; }
    public string? FileUrl { get; set; }
    public long StatusId { get; set; }
    public string StatusCode { get; set; } = string.Empty;
    public string StatusName { get; set; } = string.Empty;
    public string StatusColor { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}