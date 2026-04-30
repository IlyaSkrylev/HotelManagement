using HotelManagement.API.Common;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using HotelManagement.Application.Features.Departments;
using MediatR;

namespace HotelManagement.API.Features.Departments;

public class UpdateDepartmentEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/departments/{id}", async (
                long id,
                UpdateDepartmentDto dto,
                IMediator mediator = null!,
                ILogger<UpdateDepartmentEndpoint> logger = null!) =>
        {
            logger.LogInformation("PUT /api/departments/{Id} вызван", id);
            var command = new UpdateDepartmentCommand(id, dto.Name, dto.Description, dto.ManagerId);
            var result = await mediator.Send(command);
            return Results.Ok(BaseResponse.Ok(result));
        })
        .WithName("UpdateDepartment")
        .WithDescription("Обновление информации об отделе")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}