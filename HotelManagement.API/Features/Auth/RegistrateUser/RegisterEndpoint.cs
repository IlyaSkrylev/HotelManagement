using HotelManagement.API.Common;
using HotelManagement.Application.Features.Auth;
using MediatR;

namespace HotelManagement.API.Features.Auth.RegistrateUser;

public class RegisterEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/register", async (
                RegisterCommand command,
                IMediator mediator,
                ILogger<RegisterEndpoint> logger) =>
        {
            logger.LogInformation("POST /api/auth/register вызван");
            var result = await mediator.Send(command);

            return Results.Ok(BaseResponse.Ok(new
            {
                result.Id,
                result.Email,
                result.FirstName,
                result.LastName,
                AccessToken = result.AccessToken,
                RefreshToken = result.RefreshToken
            }, "Регистрация прошла успешно"));
        })
            .WithName("Register")
            .WithDescription("Регистрация нового пользователя")
            .Accepts<RegisterCommand>("application/json")
            .Produces<BaseResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .AllowAnonymous();
    }
}