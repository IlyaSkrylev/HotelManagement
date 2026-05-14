using HotelManagement.API.Common;
using HotelManagement.Application.Abstractions;
using HotelManagement.Application.Common;
using HotelManagement.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.API.Features.Tasks;

public class GetEmployeesForTaskEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/hotels/{hotelId}/tasks/employees", async (
                long hotelId,
                long? departmentId,
                string? searchTerm,
                IApplicationDbContext context = null!,
                ILogger<GetEmployeesForTaskEndpoint> logger = null!) =>
        {
            logger.LogInformation("GET /api/hotels/{HotelId}/tasks/employees вызван", hotelId);

            var query = context.Employees
                .Include(e => e.User)
                .Where(e => e.HotelId == hotelId && e.IsActive);

            if (departmentId.HasValue)
            {
                query = query.Where(e => e.DepartmentId == departmentId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.Trim().ToLower();
                query = query.Where(e =>
                    e.User.FirstName.ToLower().Contains(search) ||
                    e.User.LastName.ToLower().Contains(search) ||
                    (e.User.Patronymic != null && e.User.Patronymic.ToLower().Contains(search)));
            }

            var employees = await query
                .OrderBy(e => e.User.LastName)
                .ThenBy(e => e.User.FirstName)
                .Select(e => new EmployeeForSelectDto
                {
                    Id = e.Id,
                    UserId = e.UserId,
                    FullName = e.User.LastName + " " + e.User.FirstName + (e.User.Patronymic != null ? " " + e.User.Patronymic : ""),
                    Position = e.Position,
                    AvatarUrl = e.User.AvatarUrl
                })
                .ToListAsync();

            return Results.Ok(BaseResponse.Ok(employees));
        })
        .WithName("GetEmployeesForTask")
        .WithDescription("Получение списка сотрудников для выбора исполнителя задачи")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .RequireAuthorization();
    }
}