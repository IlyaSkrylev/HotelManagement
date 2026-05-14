using HotelManagement.Application.Abstractions;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using HotelManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.Tasks;

public record GetMyTasksQuery(
    long HotelId,
    long EmployeeId,
    bool IncludeInactive = false,
    long? PriorityId = null,
    int Page = 1,
    int PageSize = 20) : IRequest<PaginatedResult<TaskDto>>;

public class GetMyTasksQueryHandler : IRequestHandler<GetMyTasksQuery, PaginatedResult<TaskDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetMyTasksQueryHandler> _logger;

    public GetMyTasksQueryHandler(IApplicationDbContext context, ILogger<GetMyTasksQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PaginatedResult<TaskDto>> Handle(GetMyTasksQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Запрос задач сотрудника ID: {EmployeeId}, IncludeInactive: {IncludeInactive}",
            request.EmployeeId, request.IncludeInactive);

        var query = _context.Tasks
            .Include(t => t.TaskType)
            .Include(t => t.TaskStatus)
            .Include(t => t.Priority)
            .Include(t => t.AssignedTo).ThenInclude(e => e.User)
            .Include(t => t.CreatedBy).ThenInclude(e => e.User)
            .Include(t => t.Room)
            .Where(t => t.AssignedToId == request.EmployeeId && t.AssignedTo.HotelId == request.HotelId);

        query = query.Where(t => t.IsActive == !request.IncludeInactive);

        if (request.PriorityId.HasValue)
        {
            query = query.Where(t => t.PriorityId == request.PriorityId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        IOrderedQueryable<HotelTask> orderedQuery;
        if (request.IncludeInactive)
        {
            orderedQuery = query.OrderByDescending(t => t.CompletedAt);
        }
        else
        {
            orderedQuery = query
                .OrderByDescending(t => t.Priority.Level)
                .ThenBy(t => t.CreatedAt);
        }

        var items = await orderedQuery
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => new TaskDto
            {
                Id = t.Id,
                TaskTypeId = t.TaskTypeId,
                TaskTypeName = t.TaskType.Name,
                TaskStatusId = t.TaskStatusId,
                TaskStatusName = t.TaskStatus.Name,
                TaskStatusColor = t.TaskStatus.Color,
                TaskStatusCode = t.TaskStatus.Code,
                PriorityId = t.PriorityId,
                PriorityName = t.Priority.Name,
                PriorityColor = t.Priority.Color,
                AssignedToId = t.AssignedToId,
                AssignedToName = t.AssignedTo.User.LastName + " " + t.AssignedTo.User.FirstName,
                CreatedById = t.CreatedById,
                CreatedByName = t.CreatedBy.User.LastName + " " + t.CreatedBy.User.FirstName,
                RoomId = t.RoomId,
                RoomNumber = t.Room != null ? t.Room.RoomNumber : null,
                DueDate = t.DueDate,
                CompletedAt = t.CompletedAt,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                Notes = t.Notes,
                RequiresInspection = t.RequiresInspection,
                IsActive = t.IsActive
            })
            .ToListAsync(cancellationToken);

        return new PaginatedResult<TaskDto>(items, totalCount, request.Page, request.PageSize);
    }
}