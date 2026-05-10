using HotelManagement.Application.Abstractions;
using HotelManagement.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.Hotels;

public record GetUserRoleInHotelQuery(long UserId, long HotelId) : IRequest<UserRoleDto?>;

public class GetUserRoleInHotelQueryHandler : IRequestHandler<GetUserRoleInHotelQuery, UserRoleDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetUserRoleInHotelQueryHandler> _logger;

    public GetUserRoleInHotelQueryHandler(IApplicationDbContext context, ILogger<GetUserRoleInHotelQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<UserRoleDto?> Handle(GetUserRoleInHotelQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Запрос роли пользователя {UserId} в отеле {HotelId}", request.UserId, request.HotelId);

        var userHotelRole = await _context.UserHotelRoles
            .Include(uhr => uhr.Role)
            .FirstOrDefaultAsync(uhr => uhr.UserId == request.UserId && uhr.HotelId == request.HotelId, cancellationToken);

        if (userHotelRole == null)
        {
            _logger.LogWarning("Роль не найдена для пользователя {UserId} в отеле {HotelId}", request.UserId, request.HotelId);
            return null;
        }

        return new UserRoleDto
        {
            Id = userHotelRole.Role.Id,
            Code = userHotelRole.Role.Code,
            Name = userHotelRole.Role.Name,
            Description = userHotelRole.Role.Description
        };
    }
}