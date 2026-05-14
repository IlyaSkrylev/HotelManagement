namespace HotelManagement.Application.DTOs;

public class EmployeeScheduleDto
{
    public long EmployeeId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public List<DailyScheduleDto> Schedule { get; set; } = new();
    public ShiftTypeInfoDto ShiftType { get; set; } = null!;

    public DateTimeOffset? VacationStartDate { get; set; }
    public DateTimeOffset? VacationEndDate { get; set; }
    public string? VacationType { get; set; }
}

public class DailyScheduleDto
{
    public DateTime Date { get; set; }
    public string ShiftType { get; set; } = string.Empty; 
    public string ShiftTypeCode { get; set; } = string.Empty;
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public bool IsOnVacation { get; set; }
    public string? VacationType { get; set; }
}

public class ShiftTypeInfoDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DayShiftStart { get; set; } = string.Empty;
    public string DayShiftEnd { get; set; } = string.Empty;
    public string NightShiftStart { get; set; } = string.Empty;
    public string NightShiftEnd { get; set; } = string.Empty;
}