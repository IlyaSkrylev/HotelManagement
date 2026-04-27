using HotelManagement.API.Common;
using HotelManagement.Application.Features.Profile;
using MediatR;

namespace HotelManagement.API.Features.Profile;

public class GetProfileEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/profile", async (
                IMediator mediator,
                ILogger<GetProfileEndpoint> logger) =>
        {
            logger.LogInformation("GET /api/profile вызван");
            var result = await mediator.Send(new GetProfileQuery());
            return Results.Ok(BaseResponse.Ok(result));
        })
            .WithName("GetProfile")
            .WithDescription("Получение профиля текущего пользователя")
            .Produces<BaseResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();
    }
}