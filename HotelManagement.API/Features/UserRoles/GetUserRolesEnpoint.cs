using HotelManagement.API.Common;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using HotelManagement.Application.Features.UserRoles;
using MediatR;

namespace HotelManagement.API.Features.UserRoles;

public class GetUserRolesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/user-roles", async (
                IMediator mediator,
                ILogger<GetUserRolesEndpoint> logger) =>
        {
            logger.LogInformation("GET /api/user-roles вызван");

            var query = new GetUserRolesQuery();
            var result = await mediator.Send(query);

            return Results.Ok(BaseResponse.Ok(result));
        })
        .WithName("GetUserRoles")
        .WithDescription("Получение списка ролей пользователей")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}