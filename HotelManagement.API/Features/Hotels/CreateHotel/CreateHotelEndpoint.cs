using HotelManagement.API.Common;
using HotelManagement.Application.Features.Hotels;
using MediatR;

namespace HotelManagement.API.Features.Hotels.CreateHotel
{
    public class CreateHotelEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/hotels", async (
                    CreateHotelCommand command,
                    IMediator mediator,
                    ILogger<CreateHotelEndpoint> logger) =>
                    {
                        logger.LogInformation("POST /api/hotels вызван для создания гостиницы {Name}", command.Name);
                        var result = await mediator.Send(command);
                        return Results.Created($"/api/hotels/{result.Id}", BaseResponse.Ok(result, "Гостиница успешно создана"));
                    })
                .WithName("CreateHotel")
                .WithDescription("Создаёт новую гостиницу")
                .Accepts<CreateHotelCommand>("application/json")
                .Produces<BaseResponse>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .RequireAuthorization();
        }
    }
}
