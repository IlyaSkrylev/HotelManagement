using HotelManagement.API.Common;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using HotelManagement.Application.Features.Employees;
using MediatR;

namespace HotelManagement.API.Features.Employees;

public class EndWorkShiftEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/employees/{employeeId}/schedule/end-shift", async (
                long employeeId,
                IMediator mediator,
                ILogger<EndWorkShiftEndpoint> logger) =>
        {
            logger.LogInformation("POST /api/employees/{EmployeeId}/schedule/end-shift вызван", employeeId);
            var command = new EndWorkShiftCommand(employeeId);
            var result = await mediator.Send(command);
            return Results.Ok(BaseResponse.Ok(result));
        })
        .WithName("EndWorkShift")
        .WithDescription("Завершение рабочей смены сотрудника")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .RequireAuthorization();
    }
}