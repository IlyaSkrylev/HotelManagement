using HotelManagement.Application.Abstractions;
using HotelManagement.Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.Departments;

public record UpdateDepartmentCommand(
    long Id,
    string Name,
    string? Description,
    long? ManagerId) : IRequest<DepartmentDto>;

public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, DepartmentDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<UpdateDepartmentCommandHandler> _logger;

    public UpdateDepartmentCommandHandler(IApplicationDbContext context, ILogger<UpdateDepartmentCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<DepartmentDto> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Обновление отдела ID: {Id}", request.Id);

        var normalizedName = request.Name.Trim();

        var department = await _context.Departments
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);

        if (department == null)
        {
            throw new Exception("Отдел не найден");
        }

        var nameExists = await _context.Departments
            .AnyAsync(d => d.HotelId == department.HotelId &&
                           d.Id != department.Id &&
                           d.Name.ToLower() == normalizedName.ToLower(), cancellationToken);

        if (nameExists)
        {
            throw new InvalidOperationException("Отдел с таким названием уже существует в этой гостинице");
        }

        department.Name = normalizedName;
        department.Description = request.Description;
        department.ManagerId = request.ManagerId;
        department.UpdatedAt = DateTimeOffset.UtcNow;

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogWarning(ex, "Ошибка обновления отдела ID: {Id}, нарушение уникальности имени", request.Id);
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