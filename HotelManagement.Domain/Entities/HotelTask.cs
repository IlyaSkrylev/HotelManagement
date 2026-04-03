using HotelManagement.Domain.Entities.Abstractions;

namespace HotelManagement.Domain.Entities;

public class HotelTask : IEntity
{
    public long Id { get; set; }
    public long TaskTypeId { get; set; }
    public long TaskStatusId { get; set; }
    public long PriorityId { get; set; }
    public long AssignedToId { get; set; }
    public long CreatedById { get; set; }
    public long? RoomId { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public string? Notes { get; set; }

    public virtual HotelTaskType TaskType { get; set; } = null!;
    public virtual HotelTaskStatus TaskStatus { get; set; } = null!;
    public virtual TaskPriority Priority { get; set; } = null!;
    public virtual Employee AssignedTo { get; set; } = null!;
    public virtual Employee CreatedBy { get; set; } = null!;
    public virtual HotelRoom? Room { get; set; }
}