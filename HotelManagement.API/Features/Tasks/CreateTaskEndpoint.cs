using HotelManagement.API.Common;
using HotelManagement.Application.Abstractions;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using HotelManagement.Application.Features.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.API.Features.Tasks;

public class CreateTaskEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/hotels/{hotelId}/tasks", async (
                long hotelId,
                CreateTaskDto dto,
                IMediator mediator = null!,
                ICurrentUserService currentUserService = null!,
                IApplicationDbContext context = null!,
                ILogger<CreateTaskEndpoint> logger = null!) =>
        {
            logger.LogInformation("POST /api/hotels/{HotelId}/tasks вызван", hotelId);

            var currentEmployee = await context.Employees
                .FirstOrDefaultAsync(e => e.UserId == currentUserService.UserId && e.HotelId == hotelId)
                .ConfigureAwait(false);

            if (currentEmployee == null)
            {
                return Results.BadRequest(BaseResponse.Error("Сотрудник не найден"));
            }

            var command = new CreateTaskCommand(
                hotelId,
                dto.TaskTypeId,
                dto.NewTaskTypeName,
                dto.PriorityId,
                dto.AssignedToId,
                dto.RoomId,
                dto.DueDate,
                dto.Notes,
                dto.RequiresInspection,
                currentEmployee.Id);

            var result = await mediator.Send(command).ConfigureAwait(false);
            return Results.Ok(BaseResponse.Ok(result));
        })
        .WithName("CreateTask")
        .WithDescription("Создание новой задачи")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}