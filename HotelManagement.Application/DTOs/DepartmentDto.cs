namespace HotelManagement.Application.DTOs;

public class DepartmentDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public long? ManagerId { get; set; }
    public string? ManagerName { get; set; }
    public string? ManagerPosition { get; set; }
    public int EmployeeCount { get; set; }
    public long HotelId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public class CreateDepartmentDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public long? ManagerId { get; set; }
}

public class UpdateDepartmentDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public long? ManagerId { get; set; }
}

public class EmployeeForSelectDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string RoleCode { get; set; } = string.Empty; 
}