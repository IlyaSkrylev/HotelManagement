namespace HotelManagement.Application.DTOs;

public class TaskDto
{
    public long Id { get; set; }
    public long TaskTypeId { get; set; }
    public string TaskTypeName { get; set; } = string.Empty;
    public long TaskStatusId { get; set; }
    public string? TaskStatusCode { get; set; }
    public string TaskStatusName { get; set; } = string.Empty;
    public string? TaskStatusColor { get; set; }
    public long PriorityId { get; set; }
    public string PriorityName { get; set; } = string.Empty;
    public string? PriorityColor { get; set; }
    public long AssignedToId { get; set; }
    public string AssignedToName { get; set; } = string.Empty;
    public long CreatedById { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public long? RoomId { get; set; }
    public string? RoomNumber { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public string? Notes { get; set; }
    public bool RequiresInspection { get; set; }
    public bool IsActive { get; set; }
}

public class CreateTaskDto
{
    public long? TaskTypeId { get; set; }  
    public string? NewTaskTypeName { get; set; }
    public long PriorityId { get; set; }
    public long AssignedToId { get; set; }
    public long? RoomId { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    public string? Notes { get; set; }
    public bool RequiresInspection { get; set; }
}

public class UpdateTaskDto
{
    public long? TaskTypeId { get; set; }
    public string? NewTaskTypeName { get; set; }
    public long? TaskStatusId { get; set; }
    public long? PriorityId { get; set; }
    public long? AssignedToId { get; set; }
    public long? RoomId { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    public string? Notes { get; set; }
    public bool? RequiresInspection { get; set; }
}

public class TaskTypeDto
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public long DepartmentId { get; set; }
}

public class TaskPriorityDto
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int? Level { get; set; }
    public string? Color { get; set; }
}

public class TaskStatusDto
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }
    public string? Description { get; set; }
}