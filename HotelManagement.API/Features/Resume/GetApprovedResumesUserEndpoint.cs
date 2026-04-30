using HotelManagement.API.Common;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using HotelManagement.Application.Features.Resumes;
using MediatR;

namespace HotelManagement.API.Features.Resumes;

public class GetApprovedResumesUsersEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/hotels/{hotelId}/approved-resumes", async (
                long hotelId,
                [AsParameters] GetApprovedResumesUsersQuery query,
                IMediator mediator,
                ILogger<GetApprovedResumesUsersEndpoint> logger) =>
        {
            logger.LogInformation("GET /api/hotels/{HotelId}/approved-resumes вызван", hotelId);
            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
            .WithName("GetApprovedResumesUsers")
            .WithDescription("Получение списка пользователей с одобренными резюме для отеля")
            .Produces<PaginatedResult<ApprovedResumeUserDto>>(StatusCodes.Status200OK)
            .RequireAuthorization();
    }
}