using HotelManagement.API.Common;
using HotelManagement.Application.Features.Profile;
using MediatR;
using System.Globalization;

namespace HotelManagement.API.Features.Profile;

public class UpdateProfileEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/profile", async (
                HttpRequest request,
                IMediator mediator,
                ILogger<UpdateProfileEndpoint> logger) =>
        {
            var form = await request.ReadFormAsync();
            DateTime? birthDate = null;
            var birthDateRaw = form["BirthDate"].ToString();

            if (!string.IsNullOrWhiteSpace(birthDateRaw))
            {
                if (DateTime.TryParseExact(
                        birthDateRaw,
                        "yyyy-MM-dd",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out var parsedBirthDate) ||
                    DateTime.TryParse(
                        birthDateRaw,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                        out parsedBirthDate))
                {
                    birthDate = DateTime.SpecifyKind(parsedBirthDate, DateTimeKind.Utc);
                }
                else
                {
                    return Results.BadRequest(BaseResponse.Error("Некорректный формат даты рождения"));
                }
            }

            var command = new UpdateProfileCommand(
                FirstName: form["FirstName"].ToString(),
                LastName: form["LastName"].ToString(),
                Patronymic: form["Patronymic"].ToString(),
                Phone: form["Phone"].ToString(),
                BirthDate: birthDate,
                Avatar: form.Files.GetFile("Avatar")
            );

            logger.LogInformation("PUT /api/profile вызван");
            var result = await mediator.Send(command);
            return Results.Ok(BaseResponse.Ok(result, "Профиль обновлён"));
        })
            .WithName("UpdateProfile")
            .WithDescription("Обновление профиля пользователя")
            .Accepts<UpdateProfileCommand>("multipart/form-data")
            .Produces<BaseResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();
    }
}