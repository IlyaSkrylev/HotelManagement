using HotelManagement.API.Common;
using HotelManagement.Application.DTOs;
using HotelManagement.Application.Features.FinancialOperations;
using MediatR;

namespace HotelManagement.API.Features.Financial;

public class GetDepartmentFinancialOperationsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/financial/department/{departmentId}", async (
                long departmentId,
                int page,
                int pageSize,
                IMediator mediator) =>
        {
            var query = new GetDepartmentFinancialOperationsQuery(departmentId, page, pageSize);
            var result = await mediator.Send(query);
            return Results.Ok(BaseResponse.Ok(result));
        })
        .WithName("GetDepartmentFinancialOperations")
        .WithDescription("Получение финансовых операций отдела")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}