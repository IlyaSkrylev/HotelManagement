namespace HotelManagement.Application.DTOs;

public class HotelAdminInfoDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string UserRole { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
}