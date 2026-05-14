using HotelManagement.Application.Abstractions;
using HotelManagement.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.Tasks;

public record GetTaskStatusesQuery : IRequest<List<TaskStatusDto>>;

public class GetTaskStatusesQueryHandler : IRequestHandler<GetTaskStatusesQuery, List<TaskStatusDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetTaskStatusesQueryHandler> _logger;

    public GetTaskStatusesQueryHandler(IApplicationDbContext context, ILogger<GetTaskStatusesQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<TaskStatusDto>> Handle(GetTaskStatusesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Запрос статусов задач");

        var statuses = await _context.TaskStatuses
            .OrderBy(s => s.Id)
            .Select(s => new TaskStatusDto
            {
                Id = s.Id,
                Code = s.Code,
                Name = s.Name,
                Color = s.Color,
                Description = s.Description
            })
            .ToListAsync(cancellationToken);

        return statuses;
    }
}