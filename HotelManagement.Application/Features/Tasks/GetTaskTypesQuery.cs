using HotelManagement.Application.Abstractions;
using HotelManagement.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.Tasks;

public record GetTaskTypesQuery(long? DepartmentId = null) : IRequest<List<TaskTypeDto>>;

public class GetTaskTypesQueryHandler : IRequestHandler<GetTaskTypesQuery, List<TaskTypeDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetTaskTypesQueryHandler> _logger;

    public GetTaskTypesQueryHandler(IApplicationDbContext context, ILogger<GetTaskTypesQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<TaskTypeDto>> Handle(GetTaskTypesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Запрос типов задач для отдела ID: {DepartmentId}", request.DepartmentId);

        var query = _context.TaskTypes.AsQueryable();

        if (request.DepartmentId.HasValue)
        {
            query = query.Where(tt => tt.DepartmentId == request.DepartmentId.Value);
        }

        var taskTypes = await query
            .OrderBy(tt => tt.Name)
            .Select(tt => new TaskTypeDto
            {
                Id = tt.Id,
                Code = tt.Code,
                Name = tt.Name,
                DepartmentId = tt.DepartmentId
            })
            .ToListAsync(cancellationToken);

        return taskTypes;
    }
}