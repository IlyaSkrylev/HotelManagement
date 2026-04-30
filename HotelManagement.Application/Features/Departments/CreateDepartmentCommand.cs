using HotelManagement.Application.Abstractions;
using HotelManagement.Application.DTOs;
using HotelManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.Departments;

public record CreateDepartmentCommand(
    long HotelId,
    string Name,
    string? Description,
    long? ManagerId) : IRequest<DepartmentDto>;

public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, DepartmentDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CreateDepartmentCommandHandler> _logger;

    public CreateDepartmentCommandHandler(IApplicationDbContext context, ILogger<CreateDepartmentCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<DepartmentDto> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Создание отдела для отеля ID: {HotelId}", request.HotelId);

        var normalizedName = request.Name.Trim();

        var exists = await _context.Departments
            .AnyAsync(d => d.HotelId == request.HotelId && d.Name.ToLower() == normalizedName.ToLower(), cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException("Отдел с таким названием уже существует в этой гостинице");
        }

        var department = new Department
        {
            Name = normalizedName,
            Description = request.Description,
            HotelId = request.HotelId,
            ManagerId = request.ManagerId,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _context.Departments.Add(department);
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogWarning(ex, "Ошибка создания отдела: нарушение уникальности имени");
            throw new InvalidOperationException("Отдел с таким названием уже существует");
        }

        return new DepartmentDto
        {
            Id = department.Id,
            Name = department.Name,
            Description = department.Description,
            ManagerId = department.ManagerId,
            HotelId = department.HotelId,
            CreatedAt = department.CreatedAt,
            UpdatedAt = department.UpdatedAt
        };
    }
}