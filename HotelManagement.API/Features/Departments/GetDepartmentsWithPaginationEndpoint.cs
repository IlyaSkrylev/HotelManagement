using HotelManagement.API.Common;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using HotelManagement.Application.Features.Departments;
using MediatR;

namespace HotelManagement.API.Features.Departments;

public class GetDepartmentsWithPaginationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/hotels/{hotelId}/departments/paginated", async (
                long hotelId,
                string? searchTerm,
                int page = 1,
                int pageSize = 20,
                IMediator mediator = null!,
                ILogger<GetDepartmentsWithPaginationEndpoint> logger = null!) =>
        {
            logger.LogInformation("GET /api/hotels/{HotelId}/departments/paginated вызван", hotelId);
            var query = new GetDepartmentsQuery(hotelId, searchTerm, page, pageSize);
            var result = await mediator.Send(query);
            return Results.Ok(BaseResponse.Ok(result));
        })
        .WithName("GetDepartmentsWithPagination")
        .WithDescription("Получение списка отделов отеля с пагинацией и поиском")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}