using HotelManagement.Application.Abstractions;
using HotelManagement.Application.DTOs;
using HotelManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.Tasks;

public record CreateTaskCommand(
    long HotelId,
    long? TaskTypeId,
    string? NewTaskTypeName,
    long PriorityId,
    long AssignedToId,
    long? RoomId,
    DateTimeOffset? DueDate,
    string? Notes,
    bool RequiresInspection,
    long CreatedById) : IRequest<TaskDto>;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, TaskDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CreateTaskCommandHandler> _logger;

    public CreateTaskCommandHandler(IApplicationDbContext context, ILogger<CreateTaskCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<TaskDto> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Создание задачи для отеля ID: {HotelId}", request.HotelId);

        long taskTypeId;

        if (!request.TaskTypeId.HasValue && !string.IsNullOrWhiteSpace(request.NewTaskTypeName))
        {
            var assignedTo = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == request.AssignedToId, cancellationToken);

            var existingTaskType = await _context.TaskTypes
                .FirstOrDefaultAsync(tt => tt.Name.ToLower() == request.NewTaskTypeName.Trim().ToLower() &&
                                           tt.DepartmentId == assignedTo.DepartmentId, cancellationToken);

            if (existingTaskType != null)
            {
                taskTypeId = existingTaskType.Id;
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
                taskTypeId = newTaskType.Id;
            }
        }
        else if (request.TaskTypeId.HasValue)
        {
            taskTypeId = request.TaskTypeId.Value;
        }
        else
        {
            throw new Exception("Необходимо указать тип задачи");
        }

        var pendingStatus = await _context.TaskStatuses
            .FirstOrDefaultAsync(s => s.Code == "pending", cancellationToken);

        if (pendingStatus == null)
        {
            throw new Exception("Статус 'pending' не найден");
        }

        var task = new HotelTask
        {
            TaskTypeId = taskTypeId,
            TaskStatusId = pendingStatus.Id,
            PriorityId = request.PriorityId,
            AssignedToId = request.AssignedToId,
            CreatedById = request.CreatedById,
            RoomId = request.RoomId,
            DueDate = request.DueDate,
            Notes = request.Notes,
            RequiresInspection = request.RequiresInspection,
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync(cancellationToken);

        var createdTask = await _context.Tasks
            .Include(t => t.TaskType)
            .Include(t => t.TaskStatus)
            .Include(t => t.Priority)
            .Include(t => t.AssignedTo).ThenInclude(e => e.User)
            .Include(t => t.CreatedBy).ThenInclude(e => e.User)
            .Include(t => t.Room)
            .FirstOrDefaultAsync(t => t.Id == task.Id, cancellationToken);

        return MapToDto(createdTask!);
    }

    private TaskDto MapToDto(HotelTask task)
    {
        return new TaskDto
        {
            Id = task.Id,
            TaskTypeId = task.TaskTypeId,
            TaskTypeName = task.TaskType.Name,
            TaskStatusId = task.TaskStatusId,
            TaskStatusName = task.TaskStatus.Name,
            TaskStatusColor = task.TaskStatus.Color,
            PriorityId = task.PriorityId,
            PriorityName = task.Priority.Name,
            PriorityColor = task.Priority.Color,
            AssignedToId = task.AssignedToId,
            AssignedToName = task.AssignedTo.User.LastName + " " + task.AssignedTo.User.FirstName,
            CreatedById = task.CreatedById,
            CreatedByName = task.CreatedBy.User.LastName + " " + task.CreatedBy.User.FirstName,
            RoomId = task.RoomId,
            RoomNumber = task.Room?.RoomNumber,
            DueDate = task.DueDate,
            CompletedAt = task.CompletedAt,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt,
            Notes = task.Notes,
            RequiresInspection = task.RequiresInspection,
            IsActive = task.IsActive
        };
    }
}