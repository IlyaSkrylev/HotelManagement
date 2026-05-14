using HotelManagement.API.Common;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using HotelManagement.Application.Features.Tasks;
using MediatR;

namespace HotelManagement.API.Features.Tasks;

public class UpdateTaskEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/hotels/tasks/{id}", async (
                long id,
                UpdateTaskDto dto,
                IMediator mediator = null!,
                ILogger<UpdateTaskEndpoint> logger = null!) =>
        {
            logger.LogInformation("PUT /api/hotels/tasks/{Id} вызван", id);
            var command = new UpdateTaskCommand(
                id,
                dto.TaskTypeId,
                dto.NewTaskTypeName,
                dto.TaskStatusId,
                dto.PriorityId,
                dto.AssignedToId,
                dto.RoomId,
                dto.DueDate,
                dto.Notes,
                dto.RequiresInspection);
            var result = await mediator.Send(command);
            return Results.Ok(BaseResponse.Ok(result));
        })
        .WithName("UpdateTask")
        .WithDescription("Обновление задачи")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}