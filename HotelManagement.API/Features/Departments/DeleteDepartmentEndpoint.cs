using HotelManagement.API.Common;
using HotelManagement.Application.Common;
using HotelManagement.Application.Features.Departments;
using MediatR;

namespace HotelManagement.API.Features.Departments;

public class DeleteDepartmentEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/departments/{id}", async (
                long id,
                IMediator mediator = null!,
                ILogger<DeleteDepartmentEndpoint> logger = null!) =>
        {
            logger.LogInformation("DELETE /api/departments/{Id} вызван", id);
            var result = await mediator.Send(new DeleteDepartmentCommand(id));

            if (!result)
            {
                return Results.NotFound(BaseResponse.Error("Отдел не найден"));
            }

            return Results.Ok(BaseResponse.Ok(result));
        })
        .WithName("DeleteDepartment")
        .WithDescription("Удаление отдела")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .RequireAuthorization();
    }
}