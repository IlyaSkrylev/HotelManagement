using HotelManagement.Application.Abstractions;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.Resumes;

public record GetApprovedResumesUsersQuery(long HotelId, int Page = 1, int PageSize = 20) : IRequest<PaginatedResult<ApprovedResumeUserDto>>;

public class GetApprovedResumesUsersQueryHandler : IRequestHandler<GetApprovedResumesUsersQuery, PaginatedResult<ApprovedResumeUserDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetApprovedResumesUsersQueryHandler> _logger;

    public GetApprovedResumesUsersQueryHandler(IApplicationDbContext context, ILogger<GetApprovedResumesUsersQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PaginatedResult<ApprovedResumeUserDto>> Handle(GetApprovedResumesUsersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Запрос пользователей с одобренными резюме для отеля ID: {HotelId}", request.HotelId);

        var statusId = await _context.ResumeStatuses
            .Where(s => s.Code == "accepted")
            .Select(s => s.Id)
            .FirstOrDefaultAsync(cancellationToken);

        var query = _context.Resumes
            .Include(r => r.User)
            .Where(r => r.HotelId == request.HotelId && r.StatusId == statusId)
            .Select(r => new ApprovedResumeUserDto
            {
                Id = r.Id,
                UserId = r.UserId,
                FirstName = r.User.FirstName,
                LastName = r.User.LastName,
                Patronymic = r.User.Patronymic,
                AvatarUrl = r.User.AvatarUrl,
                DesiredPosition = r.DesiredPosition,
                CreatedAt = r.CreatedAt
            });

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<ApprovedResumeUserDto>(items, totalCount, request.Page, request.PageSize);
    }
}