using HotelManagement.Application.Abstractions;
using HotelManagement.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.Tasks;

public record GetTaskPrioritiesQuery : IRequest<List<TaskPriorityDto>>;

public class GetTaskPrioritiesQueryHandler : IRequestHandler<GetTaskPrioritiesQuery, List<TaskPriorityDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetTaskPrioritiesQueryHandler> _logger;

    public GetTaskPrioritiesQueryHandler(IApplicationDbContext context, ILogger<GetTaskPrioritiesQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<TaskPriorityDto>> Handle(GetTaskPrioritiesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Запрос приоритетов задач");

        var priorities = await _context.TaskPriorities
            .OrderBy(p => p.Level)
            .Select(p => new TaskPriorityDto
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                Level = p.Level,
                Color = p.Color
            })
            .ToListAsync(cancellationToken);

        return priorities;
    }
}