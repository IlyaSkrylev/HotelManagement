using HotelManagement.API.Common;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using HotelManagement.Application.Features.Departments;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.API.Features.Departments;

public class CreateDepartmentEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/hotels/{hotelId}/departments", async (
                long hotelId,
                CreateDepartmentDto dto,
                IMediator mediator = null!,
                ILogger<CreateDepartmentEndpoint> logger = null!) =>
        {
            logger.LogInformation("POST /api/hotels/{HotelId}/departments вызван", hotelId);
            try
            {
                var command = new CreateDepartmentCommand(hotelId, dto.Name, dto.Description, dto.ManagerId);
                var result = await mediator.Send(command);
                return Results.Ok(BaseResponse.Ok(result));
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(BaseResponse.Error(ex.Message));
            }
            catch (DbUpdateException)
            {
                return Results.BadRequest(BaseResponse.Error("Не удалось сохранить отдел. Возможно, отдел с таким названием уже существует."));
            }
        })
        .WithName("CreateDepartment")
        .WithDescription("Создание нового отдела в отеле")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}