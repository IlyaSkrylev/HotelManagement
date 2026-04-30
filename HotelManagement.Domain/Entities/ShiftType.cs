using HotelManagement.Domain.Entities.Abstractions;

namespace HotelManagement.Domain.Entities;

public class ShiftType : IEntity
{
    public long Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? Color { get; set; }
    public string? Description { get; set; }
    public int TotalCycleDays { get; set; }
    public int WorkingDayShifts { get; set; }
    public int WorkingNightShifts { get; set; }
    public int RestDays { get; set; }
    public TimeOnly DayShiftStartTime { get; set; }
    public TimeOnly DayShiftEndTime { get; set; }
    public TimeOnly NightShiftStartTime { get; set; }
    public TimeOnly NightShiftEndTime { get; set; }
}