namespace HotelManagement.Application.DTOs;

public class MyHotelDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? Position { get; set; }
    public long RoleId { get; set; }
    public string RoleCode { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public DateTimeOffset AssignedAt { get; set; }
}