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
    public DateTimeOffset? DismissalDate { get; set; }     
    public string? DismissalReason { get; set; }
    public long RoleId { get; set; }
    public string RoleCode { get; set; } = string.Empty;
    public int? Salary { get; set; }
    public int? SalarySupplement { get; set; }
    public bool IsActive { get; set; }
    public int WorkingDayShifts { get; set; }
    public int WorkingNightShifts { get; set; }
    public int RestDays { get; set; }
    public string DayShiftStart { get; set; } = "09:00";
    public string DayShiftEnd { get; set; } = "18:00";
    public string NightShiftStart { get; set; } = "21:00";
    public string NightShiftEnd { get; set; } = "06:00";
    public bool ShiftCycleStartsWithDay { get; set; } = true;
    public DateTimeOffset ShiftCycleStartDate { get; set; }
    public int TotalCycleDays { get; set; }
}