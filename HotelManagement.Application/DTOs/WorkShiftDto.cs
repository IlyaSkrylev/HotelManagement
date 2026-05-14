namespace HotelManagement.Application.DTOs;

public class WorkShiftDto
{
    public long Id { get; set; }
    public long EmployeeId { get; set; }
    public DateTimeOffset ShiftDate { get; set; }
    public DateTimeOffset? StartTime { get; set; }
    public DateTimeOffset? EndTime { get; set; }
    public bool IsActive => StartTime.HasValue && !EndTime.HasValue;
}