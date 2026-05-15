using HotelManagement.API.Common;
using HotelManagement.Application.DTOs;
using HotelManagement.Application.Features.FinancialOperations;
using MediatR;

namespace HotelManagement.API.Features.Financial;

public class PayDepartmentSalaryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/financial/department/{departmentId}/pay", async (
                long departmentId,
                PayDepartmentRequest request,
                IMediator mediator) =>
        {
            var command = new CreateDepartmentSalaryCommand(
                departmentId,
                request.CreatedById,
                request.IsSalary);
            var result = await mediator.Send(command);
            return Results.Ok(BaseResponse.Ok(result));
        })
        .WithName("PayDepartmentSalary")
        .WithDescription("Выплата зарплаты или премии всем сотрудникам отдела")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .RequireAuthorization();
    }
}

public class PayDepartmentRequest
{
    public long CreatedById { get; set; }
    public bool IsSalary { get; set; }
}