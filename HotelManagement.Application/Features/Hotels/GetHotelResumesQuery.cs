using HotelManagement.Application.Abstractions;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.Resumes;

public record GetHotelResumesQuery(
    long HotelId,
    long? StatusId = null,
    string? SearchTerm = null,
    int Page = 1,
    int PageSize = 20) : IRequest<PaginatedResult<ResumeListItemDto>>;

public class GetHotelResumesQueryHandler : IRequestHandler<GetHotelResumesQuery, PaginatedResult<ResumeListItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetHotelResumesQueryHandler> _logger;

    public GetHotelResumesQueryHandler(IApplicationDbContext context, ILogger<GetHotelResumesQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PaginatedResult<ResumeListItemDto>> Handle(GetHotelResumesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Запрос резюме для отеля ID: {HotelId}, StatusId: {StatusId}, SearchTerm: {SearchTerm}, Page: {Page}, PageSize: {PageSize}",
            request.HotelId, request.StatusId, request.SearchTerm, request.Page, request.PageSize);

        // Получаем все резюме для отеля
        var query = _context.Resumes
            .Include(r => r.User)
            .Include(r => r.Status)
            .Where(r => r.HotelId == request.HotelId);

        // Фильтрация по статусу
        if (request.StatusId.HasValue)
        {
            query = query.Where(r => r.StatusId == request.StatusId.Value);
        }

        // Поиск по ФИО
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.Trim().ToLower();
            query = query.Where(r =>
                r.User.FirstName.ToLower().Contains(searchTerm) ||
                r.User.LastName.ToLower().Contains(searchTerm) ||
                (r.User.Patronymic != null && r.User.Patronymic.ToLower().Contains(searchTerm)) ||
                (r.User.FirstName + " " + r.User.LastName).ToLower().Contains(searchTerm) ||
                (r.User.LastName + " " + r.User.FirstName).ToLower().Contains(searchTerm)
            );
        }

        // Сортировка по дате создания (новые сверху)
        query = query.OrderByDescending(r => r.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(r => new ResumeListItemDto
            {
                Id = r.Id,
                UserId = r.UserId,
                FirstName = r.User.FirstName,
                LastName = r.User.LastName,
                Patronymic = r.User.Patronymic,
                AvatarUrl = r.User.AvatarUrl,
                BirthDate = r.User.BirthDate,
                DesiredPosition = r.DesiredPosition,
                Experience = r.Experience,
                Education = r.Education,
                FileUrl = r.FileUrl,
                StatusId = r.StatusId,
                StatusName = r.Status.Name,
                StatusColor = r.Status.Color,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new PaginatedResult<ResumeListItemDto>(items, totalCount, request.Page, request.PageSize);
    }
}