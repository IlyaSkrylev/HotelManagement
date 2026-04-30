namespace HotelManagement.Application.DTOs;

public class MyResumeDto
{
    public long Id { get; set; }
    public long HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string DesiredPosition { get; set; } = string.Empty;
    public string? Experience { get; set; }
    public string? Education { get; set; }
    public string? FileUrl { get; set; }
    public long StatusId { get; set; }
    public string StatusCode { get; set; } = string.Empty;
    public string StatusName { get; set; } = string.Empty;
    public string? StatusColor { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ReviewedAt { get; set; }
}