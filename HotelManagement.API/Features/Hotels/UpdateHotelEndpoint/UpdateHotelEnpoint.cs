using HotelManagement.API.Common;
using HotelManagement.Application.Features.Hotels;
using MediatR;

namespace HotelManagement.API.Features.Hotels.UpdateHotel;

public class UpdateHotelEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/hotels/{id}", async (
                long id,
                HttpRequest request,
                IMediator mediator,
                ILogger<UpdateHotelEndpoint> logger) =>
        {
            var form = await request.ReadFormAsync();

            var command = new UpdateHotelCommand(
                Id: id,
                Name: form["Name"].ToString(),
                Address: form["Address"].ToString(),
                Phone: form["Phone"].ToString(),
                Email: form["Email"].ToString(),
                Description: form["Description"].ToString(),
                Image: form.Files.GetFile("Image")
            );

            logger.LogInformation("PUT /api/hotels/{Id} вызван", id);
            var result = await mediator.Send(command);

            if (result.Success)
            {
                return Results.Ok(BaseResponse.Ok(new { result.ImageUrl }, result.Message));
            }

            return Results.BadRequest(BaseResponse.Error(result.Message));
        })
            .WithName("UpdateHotel")
            .WithDescription("Обновление информации о гостинице")
            .Accepts<UpdateHotelCommand>("multipart/form-data")
            .Produces<BaseResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();
    }
}