using HotelManagement.Application.Abstractions;
using HotelManagement.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Application.Features.Hotels;

public record GetHotelAdminInfoQuery(long HotelId) : IRequest<HotelAdminInfoDto>;

public class GetHotelAdminInfoQueryHandler : IRequestHandler<GetHotelAdminInfoQuery, HotelAdminInfoDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetHotelAdminInfoQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<HotelAdminInfoDto> Handle(GetHotelAdminInfoQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        var hotel = await _context.Hotels
            .FirstOrDefaultAsync(h => h.Id == request.HotelId, cancellationToken);

        if (hotel == null)
            throw new Exception("Гостиница не найдена");

        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.UserId == userId && e.HotelId == request.HotelId, cancellationToken);

        return new HotelAdminInfoDto
        {
            Id = hotel.Id,
            Name = hotel.Name,
            Address = hotel.Address,
            Phone = hotel.Phone,
            Email = hotel.Email,
            Description = hotel.Description,
            ImageUrl = hotel.ImageUrl,
            UserRole = employee?.Position ?? "Сотрудник",
            DepartmentName = employee?.Department?.Name ?? "Администрация"
        };
    }
}