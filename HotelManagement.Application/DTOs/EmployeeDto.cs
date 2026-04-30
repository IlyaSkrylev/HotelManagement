// EmployeeDto.cs
namespace HotelManagement.Application.DTOs;

public class EmployeeDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string Position { get; set; } = string.Empty;
    public long DepartmentId { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset HireDate { get; set; }
}