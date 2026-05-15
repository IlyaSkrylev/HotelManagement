using HotelManagement.API.Common;
using HotelManagement.Application.DTOs;
using HotelManagement.Application.Features.FinancialOperations;
using MediatR;

namespace HotelManagement.API.Features.Financial;

public class CreateFinancialOperationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/financial", async (
                CreateFinancialOperationRequest request,
                IMediator mediator) =>
        {
            var command = new CreateFinancialOperationCommand(
                request.EmployeeId,
                request.Amount,
                request.Description,
                request.CreatedById);
            var result = await mediator.Send(command);
            return Results.Ok(BaseResponse.Ok(result));
        })
        .WithName("CreateFinancialOperation")
        .WithDescription("Создание финансовой операции для сотрудника")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .RequireAuthorization();
    }
}

public class CreateFinancialOperationRequest
{
    public long EmployeeId { get; set; }
    public int Amount { get; set; }
    public string? Description { get; set; }
    public long CreatedById { get; set; }
}