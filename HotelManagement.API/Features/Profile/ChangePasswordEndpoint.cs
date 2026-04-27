using HotelManagement.API.Common;
using HotelManagement.Application.Features.Profile;
using MediatR;

namespace HotelManagement.API.Features.Profile;

public class ChangePasswordEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/profile/change-password", async (
                ChangePasswordCommand command,
                IMediator mediator,
                ILogger<ChangePasswordEndpoint> logger) =>
        {
            logger.LogInformation("POST /api/profile/change-password вызван");
            await mediator.Send(command);
            return Results.Ok(BaseResponse.Ok(null, "Пароль успешно изменён"));
        })
            .WithName("ChangePassword")
            .WithDescription("Изменение пароля пользователя")
            .Accepts<ChangePasswordCommand>("application/json")
            .Produces<BaseResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();
    }
}