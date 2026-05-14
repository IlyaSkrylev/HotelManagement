using HotelManagement.API.Common;
using HotelManagement.Application.Common;
using HotelManagement.Application.Features.Employees;
using MediatR;

namespace HotelManagement.API.Features.Employees;

public class StartWorkShiftEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/employees/{employeeId}/schedule/start-shift", async (
                long employeeId,
                IMediator mediator,
                ILogger<StartWorkShiftEndpoint> logger) =>
        {
            logger.LogInformation("POST /api/employees/{EmployeeId}/schedule/start-shift вызван", employeeId);
            var command = new StartWorkShiftCommand(employeeId);
            var result = await mediator.Send(command);
            return Results.Ok(BaseResponse.Ok(result));
        })
        .WithName("StartWorkShift")
        .WithDescription("Начало рабочей смены сотрудника")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .RequireAuthorization();
    }
}