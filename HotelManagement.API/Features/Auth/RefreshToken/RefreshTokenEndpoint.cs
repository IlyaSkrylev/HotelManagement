using HotelManagement.API.Common;
using HotelManagement.Application.Features.Auth;
using MediatR;

namespace HotelManagement.API.Features.Auth.RefreshToken;

public class RefreshTokenEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/refresh", async (
                RefreshTokenCommand command,
                IMediator mediator,
                ILogger<RefreshTokenEndpoint> logger) =>
        {
            logger.LogInformation("POST /api/auth/refresh вызван");
            var result = await mediator.Send(command);
            return Results.Ok(BaseResponse.Ok(result, "Токен обновлён"));
        })
            .WithName("RefreshToken")
            .WithDescription("Обновление access токена")
            .Accepts<RefreshTokenCommand>("application/json")
            .Produces<BaseResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .AllowAnonymous();
    }
}