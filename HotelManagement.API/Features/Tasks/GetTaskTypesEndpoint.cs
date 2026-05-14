using HotelManagement.API.Common;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using HotelManagement.Application.Features.Tasks;
using MediatR;

namespace HotelManagement.API.Features.Tasks;

public class GetTaskTypesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/hotels/tasks/types", async (
                long? departmentId,
                IMediator mediator = null!,
                ILogger<GetTaskTypesEndpoint> logger = null!) =>
        {
            logger.LogInformation("GET /api/hotels/tasks/types вызван");
            var query = new GetTaskTypesQuery(departmentId);
            var result = await mediator.Send(query);
            return Results.Ok(BaseResponse.Ok(result));
        })
        .WithName("GetTaskTypes")
        .WithDescription("Получение списка типов задач")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}