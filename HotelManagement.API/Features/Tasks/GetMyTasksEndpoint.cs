using HotelManagement.API.Common;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using HotelManagement.Application.Features.Tasks;
using MediatR;

namespace HotelManagement.API.Features.Tasks;

public class GetMyTasksEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/hotels/{hotelId}/employees/{employeeId}/tasks", async (
                long hotelId,
                long employeeId,
                bool includeInactive = false,
                long? priorityId = null,
                int page = 1,
                int pageSize = 20,
                IMediator mediator = null!,
                ILogger<GetMyTasksEndpoint> logger = null!) =>
        {
            logger.LogInformation("GET /api/hotels/{HotelId}/employees/{EmployeeId}/tasks вызван", hotelId, employeeId);
            var query = new GetMyTasksQuery(hotelId, employeeId, includeInactive, priorityId, page, pageSize);
            var result = await mediator.Send(query);
            return Results.Ok(BaseResponse.Ok(result));
        })
        .WithName("GetMyTasks")
        .WithDescription("Получение списка задач сотрудника")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}