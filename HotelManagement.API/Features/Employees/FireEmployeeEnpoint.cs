using HotelManagement.API.Common;
using HotelManagement.Application.Features.Employees;
using MediatR;

namespace HotelManagement.API.Features.Employees;

public class FireEmployeeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/employees/{employeeId}", async (
                long employeeId,
                string? dismissalReason,
                IMediator mediator,
                ILogger<FireEmployeeEndpoint> logger) =>
        {
            logger.LogInformation("DELETE /api/employees/{EmployeeId} вызван", employeeId);

            var command = new FireEmployeeCommand(employeeId, dismissalReason);
            var result = await mediator.Send(command);

            if (result)
                return Results.Ok(BaseResponse.Ok(null, "Сотрудник уволен"));

            return Results.NotFound(BaseResponse.Error("Сотрудник не найден"));
        })
        .WithName("FireEmployee")
        .WithDescription("Увольнение сотрудника")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .RequireAuthorization();
    }
}