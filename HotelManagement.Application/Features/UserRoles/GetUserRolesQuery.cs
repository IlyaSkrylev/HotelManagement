using HotelManagement.Application.Abstractions;
using HotelManagement.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.UserRoles;

public record GetUserRolesQuery : IRequest<List<UserRoleDto>>;

public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, List<UserRoleDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetUserRolesQueryHandler> _logger;

    public GetUserRolesQueryHandler(IApplicationDbContext context, ILogger<GetUserRolesQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<UserRoleDto>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Запрос списка ролей пользователей");

        var roles = await _context.UserRoles
            .OrderBy(r => r.Id)
            .Select(r => new UserRoleDto
            {
                Id = r.Id,
                Code = r.Code,
                Name = r.Name,
                Description = r.Description
            })
            .ToListAsync(cancellationToken);

        return roles;
    }
}