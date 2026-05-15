using HotelManagement.API.Common;
using HotelManagement.Application.DTOs;
using HotelManagement.Application.Features.FinancialOperations;
using MediatR;

namespace HotelManagement.API.Features.Financial;

public class GetEmployeeFinancialStatsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/financial/employee/{employeeId}/stats", async (
        long employeeId,
        string? startDate,
        string? endDate,
        IMediator mediator) =>
        {
            try
            {
                var query = new GetEmployeeFinancialStatsQuery(employeeId, startDate, endDate);
                var result = await mediator.Send(query);
                return Results.Ok(BaseResponse.Ok(result));
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
            .WithName("GetEmployeeFinancialStats")
            .WithDescription("Получение статистики финансов сотрудника за период")
            .Produces<BaseResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization();
    }
}