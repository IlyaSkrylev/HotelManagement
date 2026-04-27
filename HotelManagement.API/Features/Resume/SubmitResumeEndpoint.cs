using HotelManagement.API.Common;
using HotelManagement.Application.Features.Resume;
using MediatR;

namespace HotelManagement.API.Features.Resume;

public class SubmitResumeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/resume/submit", async (
                HttpRequest request,
                IMediator mediator,
                ILogger<SubmitResumeEndpoint> logger) =>
        {
            var form = await request.ReadFormAsync();

            var useProfileResume = form["UseProfileResume"].ToString().ToLower() == "true";

            var command = new SubmitResumeCommand(
                HotelId: long.Parse(form["HotelId"].ToString()),
                DesiredPosition: form["DesiredPosition"].ToString(),
                Experience: form["Experience"].ToString(),
                Education: form["Education"].ToString(),
                UseProfileResume: useProfileResume,
                ResumeFile: form.Files.GetFile("ResumeFile")
            );

            logger.LogInformation("POST /api/resume/submit вызван");
            var result = await mediator.Send(command);
            return Results.Ok(BaseResponse.Ok(result, "Резюме успешно подано"));
        })
            .WithName("SubmitResume")
            .WithDescription("Подача резюме в гостиницу")
            .Accepts<SubmitResumeCommand>("multipart/form-data")
            .Produces<BaseResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();
    }
}