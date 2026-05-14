using HotelManagement.API.Common;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using HotelManagement.Application.Features.Tasks;
using MediatR;

namespace HotelManagement.API.Features.Tasks;

public class GetTasksEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/hotels/{hotelId}/tasks", async (
                long hotelId,
                long? departmentId = null,
                bool includeInactive = false,
                long? priorityId = null,
                long? taskStatusId = null,
                string? searchTerm = null,
                int page = 1,
                int pageSize = 20,
                IMediator mediator = null!,
                ILogger<GetTasksEndpoint> logger = null!) =>
        {
            logger.LogInformation("GET /api/hotels/{HotelId}/tasks вызван", hotelId);

            var query = new GetTasksQuery(
                hotelId,
                departmentId,
                includeInactive,
                priorityId,
                taskStatusId,
                searchTerm,
                page,
                pageSize);

            var result = await mediator.Send(query);
            return Results.Ok(BaseResponse.Ok(result));
        })
        .WithName("GetHotelTasks")
        .WithDescription("Получение списка задач отеля с пагинацией и фильтрацией")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}