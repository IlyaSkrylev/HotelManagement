using HotelManagement.API.Common;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using HotelManagement.Application.Features.Resumes;
using MediatR;

namespace HotelManagement.API.Features.Resumes;

public class GetHotelResumesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/hotels/{hotelId}/resumes", async (
                long hotelId,
                long? statusId,
                string? searchTerm,
                int page = 1,
                int pageSize = 20,
                IMediator mediator = null!,
                ILogger<GetHotelResumesEndpoint> logger = null!) =>
        {
            logger.LogInformation("GET /api/hotels/{HotelId}/resumes вызван с statusId={StatusId}, searchTerm={SearchTerm}, page={Page}, pageSize={PageSize}",
                hotelId, statusId, searchTerm, page, pageSize);

            var query = new GetHotelResumesQuery(hotelId, statusId, searchTerm, page, pageSize);
            var result = await mediator.Send(query);

            return Results.Ok(result);
        })
        .WithName("GetHotelResumes")
        .WithDescription("Получение списка резюме для отеля с пагинацией, фильтрацией по статусу и поиском по ФИО")
        .Produces<PaginatedResult<ResumeListItemDto>>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}