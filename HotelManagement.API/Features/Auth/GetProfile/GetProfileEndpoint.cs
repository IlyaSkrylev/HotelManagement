using HotelManagement.API.Common;
using HotelManagement.Application.Features.Auth;
using MediatR;

namespace HotelManagement.API.Features.Auth.GetProfile;

public class GetProfileEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/auth/profile", async (
                IMediator mediator,
                ILogger<GetProfileEndpoint> logger) =>
        {
            logger.LogInformation("GET /api/auth/profile вызван");
            var result = await mediator.Send(new GetProfileQuery());
            return Results.Ok(BaseResponse.Ok(result, "Профиль получен"));
        })
            .WithName("GetProfile")
            .WithDescription("Получение профиля текущего пользователя")
            .Produces<BaseResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();
    }
}