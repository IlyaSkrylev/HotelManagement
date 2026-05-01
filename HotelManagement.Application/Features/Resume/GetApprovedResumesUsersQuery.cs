using HotelManagement.Application.Abstractions;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.Resumes;

public record GetApprovedResumesUsersQuery(
    long HotelId,
    string? SearchTerm = null,
    int Page = 1,
    int PageSize = 20) : IRequest<PaginatedResult<ApprovedResumeUserDto>>;

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

        // Получаем статус "approved" (одобрено)
        var statusId = await _context.ResumeStatuses
            .Where(s => s.Code == "approved")  // исправлено с "accepted" на "approved"
            .Select(s => s.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (statusId == 0)
        {
            _logger.LogWarning("Статус 'approved' не найден");
            return new PaginatedResult<ApprovedResumeUserDto>(new List<ApprovedResumeUserDto>(), 0, request.Page, request.PageSize);
        }

        var query = _context.Resumes
            .Include(r => r.User)
            .Where(r => r.HotelId == request.HotelId && r.StatusId == statusId);

        // Поиск по ФИО
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.Trim().ToLower();
            query = query.Where(r =>
                r.User.FirstName.ToLower().Contains(searchTerm) ||
                r.User.LastName.ToLower().Contains(searchTerm) ||
                (r.User.Patronymic != null && r.User.Patronymic.ToLower().Contains(searchTerm)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
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
            })
            .ToListAsync(cancellationToken);

        return new PaginatedResult<ApprovedResumeUserDto>(items, totalCount, request.Page, request.PageSize);
    }
}