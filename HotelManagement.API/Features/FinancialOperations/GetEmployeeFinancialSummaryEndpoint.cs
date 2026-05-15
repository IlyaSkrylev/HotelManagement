using HotelManagement.API.Common;
using HotelManagement.Application.Abstractions;
using HotelManagement.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.API.Features.Financial;

public class GetEmployeeFinancialSummaryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/financial/employee/{employeeId}/summary", async (
                long employeeId,
                IApplicationDbContext context,
                ILogger<GetEmployeeFinancialSummaryEndpoint> logger) =>
        {
            logger.LogInformation("GET /api/financial/employee/{EmployeeId}/summary вызван", employeeId);

            var operations = await context.FinancialOperations
                .Where(fo => fo.EmployeeId == employeeId)
                .ToListAsync();

            var totalSalary = operations.Where(o => o.Description == "Зарплата").Sum(o => Math.Abs(o.Amount));
            var totalBonus = operations.Where(o => o.Description == "Премия").Sum(o => Math.Abs(o.Amount));
            var totalFine = operations.Where(o => o.Description == "Штраф").Sum(o => Math.Abs(o.Amount));
            var totalEarned = totalSalary + totalBonus - totalFine;

            return Results.Ok(BaseResponse.Ok(new
            {
                TotalSalary = totalSalary,
                TotalBonus = totalBonus,
                TotalFine = totalFine,
                TotalEarned = totalEarned
            }));
        })
        .WithName("GetEmployeeFinancialSummary")
        .WithDescription("Получение сводки по финансам сотрудника")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}