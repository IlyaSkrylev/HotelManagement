using HotelManagement.Domain.Entities.Abstractions;

namespace HotelManagement.Domain.Entities;

public class WorkShift : IEntity
{
    public long Id { get; set; }
    public long EmployeeId { get; set; }
    public long ShiftTypeId { get; set; }
    public DateTimeOffset ShiftDate { get; set; }
    public DateTimeOffset? StartTime { get; set; }
    public DateTimeOffset? EndTime { get; set; }

    public virtual Employee Employee { get; set; } = null!;
    public virtual ShiftType ShiftType { get; set; } = null!;
}