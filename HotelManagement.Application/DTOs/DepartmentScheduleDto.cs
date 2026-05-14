namespace HotelManagement.Application.DTOs;

public class DepartmentScheduleDto
{
    public long DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Month { get; set; }
    public List<DailyDepartmentScheduleDto> Schedule { get; set; } = new();
    public int TotalEmployees { get; set; }
}

public class DailyDepartmentScheduleDto
{
    public DateTime Date { get; set; }
    public List<EmployeeScheduleInfoDto> Employees { get; set; } = new();
}

public class EmployeeScheduleInfoDto
{
    public long EmployeeId { get; set; }
    public long UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string Position { get; set; } = string.Empty;
    public string ShiftType { get; set; } = string.Empty; 
    public string ShiftTypeName { get; set; } = string.Empty;
    public string? StartTime { get; set; }
    public string? EndTime { get; set; }

    public bool IsOnLeave { get; set; }
    public string? LeaveType { get; set; }
}