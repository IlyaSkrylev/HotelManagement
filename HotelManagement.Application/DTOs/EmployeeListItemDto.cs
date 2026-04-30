namespace HotelManagement.Application.DTOs;

public class EmployeeListItemDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Patronymic { get; set; }
    public string? AvatarUrl { get; set; }
    public string Position { get; set; } = string.Empty;
    public long DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public DateTimeOffset HireDate { get; set; }
    public long RoleId { get; set; }
    public string RoleCode { get; set; } = string.Empty;
}