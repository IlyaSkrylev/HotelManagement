namespace HotelManagement.Application.DTOs;

public class CurrentUserEmployeeInfoDto
{
    public long DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public string Position { get; set; } = string.Empty;
    public DateTimeOffset HireDate { get; set; }
    public bool IsActive { get; set; }
}