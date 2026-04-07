using HotelManagement.API.Common;
using HotelManagement.Application.Features.Auth;
using MediatR;

namespace HotelManagement.API.Features.Auth.LoginUser;

public class LoginEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", async (
            LoginCommand command,
            IMediator mediator,
            ILogger<LoginEndpoint> logger) =>
            {
                logger.LogInformation("POST /api/auth/login вызван");
                var result = await mediator.Send(command);
                return Results.Ok(BaseResponse.Ok(new
                {
                    result.Id,
                    result.Email,
                    result.FirstName,
                    result.LastName,
                    AccessToken = result.AccessToken,
                    RefreshToken = result.RefreshToken
                }, "Вход выполнен успешно"));
            })
            .WithName("Login")
            .WithDescription("Авторизация пользователя")
            .Accepts<LoginCommand>("application/json")
            .Produces<BaseResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .AllowAnonymous();
    }
}
