using HotelManagement.Application.Abstractions;
using HotelManagement.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
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
        _logger.LogInformation("Image is null: {IsNull}", request.Image == null);

        string? imageUrl = null;
        if (request.Image != null && request.Image.Length > 0)
        {
            _logger.LogInformation("Saving image: {FileName}, size: {Size}", request.Image.FileName, request.Image.Length);
            imageUrl = await _fileStorageService.SaveFileAsync(request.Image, "hotels", cancellationToken);
            _logger.LogInformation("Image saved to: {ImageUrl}", imageUrl);
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

        _logger.LogInformation("Гостиница создана с ID: {HotelId}", hotel.Id);

        return new CreateHotelResponse(hotel.Id, hotel.Name, hotel.ImageUrl);
    }
}