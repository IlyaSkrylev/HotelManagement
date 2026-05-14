using HotelManagement.Application.Abstractions;
using HotelManagement.Application.DTOs;
using HotelManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.Tasks;

public record UpdateTaskCommand(
    long Id,
    long? TaskTypeId,
    string? NewTaskTypeName,
    long? TaskStatusId,
    long? PriorityId,
    long? AssignedToId,
    long? RoomId,
    DateTimeOffset? DueDate,
    string? Notes,
    bool? RequiresInspection) : IRequest<TaskDto>;

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, TaskDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<UpdateTaskCommandHandler> _logger;

    public UpdateTaskCommandHandler(IApplicationDbContext context, ILogger<UpdateTaskCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<TaskDto> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Обновление задачи ID: {Id}", request.Id);

        var task = await _context.Tasks
            .Include(t => t.TaskType)
            .Include(t => t.TaskStatus)
            .Include(t => t.Priority)
            .Include(t => t.AssignedTo).ThenInclude(e => e.User)
            .Include(t => t.CreatedBy).ThenInclude(e => e.User)
            .Include(t => t.Room)
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (task == null)
        {
            throw new Exception("Задача не найдена");
        }

        if (request.TaskTypeId.HasValue || !string.IsNullOrWhiteSpace(request.NewTaskTypeName))
        {
            long newTaskTypeId;

            if (!request.TaskTypeId.HasValue && !string.IsNullOrWhiteSpace(request.NewTaskTypeName))
            {
                var assignedTo = task.AssignedTo;
                var existingTaskType = await _context.TaskTypes
                    .FirstOrDefaultAsync(tt => tt.Name.ToLower() == request.NewTaskTypeName.Trim().ToLower() &&
                                               tt.DepartmentId == assignedTo.DepartmentId, cancellationToken);

                if (existingTaskType != null)
                {
                    newTaskTypeId = existingTaskType.Id;
                }
                else
                {
                    var newTaskType = new HotelTaskType
                    {
                        Code = request.NewTaskTypeName.Trim().ToUpper().Replace(" ", "_"),
                        Name = request.NewTaskTypeName.Trim(),
                        DepartmentId = assignedTo.DepartmentId
                    };
                    _context.TaskTypes.Add(newTaskType);
                    await _context.SaveChangesAsync(cancellationToken);
                    newTaskTypeId = newTaskType.Id;
                }
            }
            else
            {
                newTaskTypeId = request.TaskTypeId.Value;
            }

            task.TaskTypeId = newTaskTypeId;
        }

        if (request.TaskStatusId.HasValue)
        {
            var oldStatus = task.TaskStatus?.Code;
            var newStatus = await _context.TaskStatuses
                .FirstOrDefaultAsync(s => s.Id == request.TaskStatusId.Value, cancellationToken);

            task.TaskStatusId = request.TaskStatusId.Value;

            if (newStatus?.Code == "completed" || newStatus?.Code == "cancelled" || newStatus?.Code == "overdue")
            {
                task.CompletedAt = DateTimeOffset.UtcNow;
                task.IsActive = false;
            }
            else if (oldStatus == "completed" || oldStatus == "cancelled" || oldStatus == "overdue")
            {
                task.CompletedAt = null;
                task.IsActive = true;
            }
        }

        if (request.PriorityId.HasValue)
        {
            task.PriorityId = request.PriorityId.Value;
        }

        if (request.AssignedToId.HasValue)
        {
            task.AssignedToId = request.AssignedToId.Value;
        }

        if (request.RoomId.HasValue)
        {
            task.RoomId = request.RoomId.Value;
        }
        else if (request.RoomId == null && task.RoomId != null)
        {
            task.RoomId = null;
        }

        if (request.DueDate.HasValue)
        {
            task.DueDate = request.DueDate.Value;
        }

        if (request.Notes != null)
        {
            task.Notes = request.Notes;
        }

        if (request.RequiresInspection.HasValue)
        {
            task.RequiresInspection = request.RequiresInspection.Value;
        }

        task.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        var updatedTask = await _context.Tasks
            .Include(t => t.TaskType)
            .Include(t => t.TaskStatus)
            .Include(t => t.Priority)
            .Include(t => t.AssignedTo).ThenInclude(e => e.User)
            .Include(t => t.CreatedBy).ThenInclude(e => e.User)
            .Include(t => t.Room)
            .FirstOrDefaultAsync(t => t.Id == task.Id, cancellationToken);

        return new TaskDto
        {
            Id = updatedTask!.Id,
            TaskTypeId = updatedTask.TaskTypeId,
            TaskTypeName = updatedTask.TaskType?.Name ?? string.Empty,
            TaskStatusId = updatedTask.TaskStatusId,
            TaskStatusName = updatedTask.TaskStatus?.Name ?? string.Empty,
            TaskStatusColor = updatedTask.TaskStatus?.Color,
            PriorityId = updatedTask.PriorityId,
            PriorityName = updatedTask.Priority?.Name ?? string.Empty,
            PriorityColor = updatedTask.Priority?.Color,
            AssignedToId = updatedTask.AssignedToId,
            AssignedToName = updatedTask.AssignedTo != null
                ? updatedTask.AssignedTo.User.LastName + " " + updatedTask.AssignedTo.User.FirstName
                : string.Empty,
            CreatedById = updatedTask.CreatedById,
            CreatedByName = updatedTask.CreatedBy != null
                ? updatedTask.CreatedBy.User.LastName + " " + updatedTask.CreatedBy.User.FirstName
                : string.Empty,
            RoomId = updatedTask.RoomId,
            RoomNumber = updatedTask.Room?.RoomNumber,
            DueDate = updatedTask.DueDate,
            CompletedAt = updatedTask.CompletedAt,
            CreatedAt = updatedTask.CreatedAt,
            UpdatedAt = updatedTask.UpdatedAt,
            Notes = updatedTask.Notes,
            RequiresInspection = updatedTask.RequiresInspection,
            IsActive = updatedTask.IsActive
        };
    }
}