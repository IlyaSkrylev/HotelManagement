using HotelManagement.API.Common;
using HotelManagement.Application.DTOs;
using HotelManagement.Application.Features.FinancialOperations;
using MediatR;

namespace HotelManagement.API.Features.Financial;

public class GetEmployeeFinancialOperationsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/financial/employee/{employeeId}", async (
                long employeeId,
        int page,
        int pageSize,
        string? startDate,
        string? endDate,
        IMediator mediator) =>
        {
            try
            {
                var query = new GetEmployeeFinancialOperationsQuery(employeeId, page, pageSize, startDate, endDate);
                var result = await mediator.Send(query);
                return Results.Ok(BaseResponse.Ok(result));
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .WithName("GetEmployeeFinancialOperations")
        .WithDescription("Получение финансовых операций сотрудника с фильтрацией по дате")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .RequireAuthorization();
    }
}