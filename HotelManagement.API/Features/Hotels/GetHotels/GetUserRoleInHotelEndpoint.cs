using HotelManagement.API.Common;
using HotelManagement.Application.Abstractions;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using HotelManagement.Application.Features.Hotels;
using MediatR;

namespace HotelManagement.API.Features.Hotels;

public class GetUserRoleInHotelEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/hotels/{hotelId}/user-role", async (
                long hotelId,
                IMediator mediator,
                ICurrentUserService currentUserService,
                ILogger<GetUserRoleInHotelEndpoint> logger) =>
        {
            logger.LogInformation("GET /api/hotels/{HotelId}/user-role вызван", hotelId);

            var query = new GetUserRoleInHotelQuery(currentUserService.UserId, hotelId);
            var result = await mediator.Send(query);

            if (result == null)
            {
                return Results.NotFound(BaseResponse.Error("Роль не найдена"));
            }

            return Results.Ok(BaseResponse.Ok(result));
        })
        .WithName("GetUserRoleInHotel")
        .WithDescription("Получение роли текущего пользователя в гостинице")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .RequireAuthorization();
    }
}