using HotelManagement.Application.Abstractions;
using HotelManagement.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.Hotels;

public record CreateHotelCommand(
    string Name,
    string Address,
    string Phone,
    string Email,
    string Description,
    IFormFile? Image
) : IRequest<CreateHotelResponse>;

public record CreateHotelResponse(long Id, string Name, string? ImageUrl);

public class CreateHotelCommandHandler : IRequestHandler<CreateHotelCommand, CreateHotelResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CreateHotelCommandHandler> _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public CreateHotelCommandHandler(
        IApplicationDbContext context,
        ILogger<CreateHotelCommandHandler> logger,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _context = context;
        _logger = logger;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<CreateHotelResponse> Handle(CreateHotelCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Создание новой гостиницы: {Name}", request.Name);

        string? imageUrl = null;
        if (request.Image != null && request.Image.Length > 0)
        {
            imageUrl = await _fileStorageService.SaveFileAsync(request.Image, "hotels", cancellationToken);
        }

        var hotel = new Hotel
        {
            Name = request.Name,
            Address = request.Address,
            Phone = request.Phone,
            Email = request.Email,
            Description = request.Description,
            ImageUrl = imageUrl,
            CreatedById = _currentUserService.UserId,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync(cancellationToken);

        var defaultDepartment = new Department
        {
            Name = "Администрация",
            HotelId = hotel.Id,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _context.Departments.Add(defaultDepartment);
        await _context.SaveChangesAsync(cancellationToken);

        var employmentTypeId = await _context.EmploymentTypes
            .OrderBy(x => x.Id)
            .Select(x => (long?)x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (!employmentTypeId.HasValue)
        {
            var defaultEmploymentType = new EmploymentType
            {
                Code = "FULL_TIME",
                Name = "Полная занятость",
                Description = "Создано автоматически при создании первой гостиницы"
            };

            _context.EmploymentTypes.Add(defaultEmploymentType);
            await _context.SaveChangesAsync(cancellationToken);
            employmentTypeId = defaultEmploymentType.Id;
        }

        var employee = new Employee
        {
            UserId = _currentUserService.UserId,
            HotelId = hotel.Id,
            DepartmentId = defaultDepartment.Id,
            Position = "Управляющий",
            HireDate = DateTimeOffset.UtcNow,
            IsActive = true,
            EmploymentTypeId = employmentTypeId.Value,
            ShiftCycleStartDate = DateTimeOffset.UtcNow,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Гостиница создана с ID: {HotelId}, сотрудник добавлен с ID: {EmployeeId}",
            hotel.Id, employee.Id);

        return new CreateHotelResponse(hotel.Id, hotel.Name, hotel.ImageUrl);
    }
}