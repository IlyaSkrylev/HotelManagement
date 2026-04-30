using HotelManagement.API.Common;
using HotelManagement.Application.Features.Resumes;
using MediatR;

namespace HotelManagement.API.Features.Resumes;

public class GetMyResumesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/profile/resumes", async (
                IMediator mediator,
                ILogger<GetMyResumesEndpoint> logger) =>
        {
            logger.LogInformation("GET /api/profile/resumes вызван");
            var result = await mediator.Send(new GetMyResumesQuery());
            return Results.Ok(BaseResponse.Ok(result));
        })
            .WithName("GetMyResumes")
            .WithDescription("Получение списка резюме текущего пользователя")
            .Produces<BaseResponse>(StatusCodes.Status200OK)
            .RequireAuthorization();
    }
}