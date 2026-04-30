using HotelManagement.Application.Abstractions;
using HotelManagement.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.Resumes;

public record GetMyResumesQuery : IRequest<List<MyResumeDto>>;

public class GetMyResumesQueryHandler : IRequestHandler<GetMyResumesQuery, List<MyResumeDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetMyResumesQueryHandler> _logger;
    private readonly ICurrentUserService _currentUserService;

    public GetMyResumesQueryHandler(
        IApplicationDbContext context,
        ILogger<GetMyResumesQueryHandler> logger,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<List<MyResumeDto>> Handle(GetMyResumesQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        _logger.LogInformation("Запрос списка резюме пользователя ID: {UserId}", userId);

        var resumes = await (
            from resume in _context.Resumes
            where resume.UserId == userId
            join hotel in _context.Hotels on resume.HotelId equals hotel.Id into hotelJoin
            from hotel in hotelJoin.DefaultIfEmpty()
            join status in _context.ResumeStatuses on resume.StatusId equals status.Id into statusJoin
            from status in statusJoin.DefaultIfEmpty()
            orderby resume.CreatedAt descending
            select new MyResumeDto
            {
                Id = resume.Id,
                HotelId = resume.HotelId,
                HotelName = hotel != null ? hotel.Name : "Гостиница удалена",
                DesiredPosition = resume.DesiredPosition,
                Experience = resume.Experience,
                Education = resume.Education,
                FileUrl = resume.FileUrl,
                StatusId = resume.StatusId,
                StatusCode = status != null ? status.Code : "unknown",
                StatusName = status != null ? status.Name : "Неизвестно",
                StatusColor = status != null ? status.Color : "#6c757d",
                CreatedAt = resume.CreatedAt,
                ReviewedAt = resume.ReviewedAt
            })
            .ToListAsync(cancellationToken);

        return resumes;
    }
}