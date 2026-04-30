using HotelManagement.Application.Abstractions;
using HotelManagement.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.Resumes;

public record GetResumeStatusesQuery : IRequest<List<ResumeStatusDto>>;

public class GetResumeStatusesQueryHandler : IRequestHandler<GetResumeStatusesQuery, List<ResumeStatusDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetResumeStatusesQueryHandler> _logger;

    public GetResumeStatusesQueryHandler(IApplicationDbContext context, ILogger<GetResumeStatusesQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<ResumeStatusDto>> Handle(GetResumeStatusesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Запрос списка статусов резюме");

        var statuses = await _context.ResumeStatuses
            .Select(s => new ResumeStatusDto
            {
                Id = s.Id,
                Code = s.Code,
                Name = s.Name,
                Color = s.Color
            })
            .ToListAsync(cancellationToken);

        return statuses;
    }
}