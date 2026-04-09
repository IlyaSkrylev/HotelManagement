using HotelManagement.API.Common;
using HotelManagement.Application.Features.Hotels;
using MediatR;

namespace HotelManagement.API.Features.Hotels.CreateHotel;

public class CreateHotelEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/hotels", async (
                IMediator mediator,
                ILogger<CreateHotelEndpoint> logger,
                HttpRequest request) =>
        {
            var form = await request.ReadFormAsync();

            logger.LogInformation("Files count: {Count}", form.Files.Count);
            if (form.Files.Count > 0)
            {
                var file = form.Files[0];
                logger.LogInformation("File name: {FileName}, size: {Size}", file.FileName, file.Length);
            }
            else
            {
                logger.LogWarning("No files received!");
            }

            var command = new CreateHotelCommand(
                Name: form["Name"].ToString(),
                Address: form["Address"].ToString(),
                Phone: form["Phone"].ToString(),
                Email: form["Email"].ToString(),
                Description: form["Description"].ToString(),
                Image: form.Files.Count > 0 ? form.Files[0] : null
            );

            logger.LogInformation("POST /api/hotels вызван для создания гостиницы {Name}", command.Name);
            var result = await mediator.Send(command);
            return Results.Created($"/api/hotels/{result.Id}", BaseResponse.Ok(result, "Гостиница успешно создана"));
        })
            .WithName("CreateHotel")
            .WithDescription("Создаёт новую гостиницу")
            .Accepts<CreateHotelCommand>("multipart/form-data")
            .Produces<BaseResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();
    }
}