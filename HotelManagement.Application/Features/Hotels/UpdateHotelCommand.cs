using HotelManagement.Application.Abstractions;
using HotelManagement.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.Hotels;

public record UpdateHotelCommand(
    long Id,
    string Name,
    string Address,
    string Phone,
    string Email,
    string Description,
    IFormFile? Image
) : IRequest<UpdateHotelResponse>;

public record UpdateHotelResponse(bool Success, string Message, string? ImageUrl);

public class UpdateHotelCommandHandler : IRequestHandler<UpdateHotelCommand, UpdateHotelResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<UpdateHotelCommandHandler> _logger;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public UpdateHotelCommandHandler(
        IApplicationDbContext context,
        ILogger<UpdateHotelCommandHandler> logger,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _context = context;
        _logger = logger;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<UpdateHotelResponse> Handle(UpdateHotelCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Обновление гостиницы ID: {HotelId}", request.Id);

        var userId = _currentUserService.UserId;

        var hotel = await _context.Hotels
            .FirstOrDefaultAsync(h => h.Id == request.Id && h.CreatedById == userId, cancellationToken);

        if (hotel == null)
        {
            _logger.LogWarning("Гостиница не найдена или пользователь не является владельцем");
            return new UpdateHotelResponse(false, "Гостиница не найдена или у вас нет прав на её редактирование", null);
        }

        hotel.Name = request.Name;
        hotel.Address = request.Address;
        hotel.Phone = request.Phone;
        hotel.Email = request.Email;
        hotel.Description = request.Description;
        hotel.UpdatedAt = DateTimeOffset.UtcNow;

        if (request.Image != null && request.Image.Length > 0)
        {
            if (!string.IsNullOrEmpty(hotel.ImageUrl))
            {
                _fileStorageService.DeleteFile(hotel.ImageUrl);
            }

            var imageUrl = await _fileStorageService.SaveFileAsync(request.Image, "hotels", cancellationToken);
            hotel.ImageUrl = imageUrl;

            _logger.LogInformation("Фотография обновлена: {ImageUrl}", imageUrl);
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Гостиница ID: {HotelId} успешно обновлена", hotel.Id);

        return new UpdateHotelResponse(true, "Гостиница успешно обновлена", hotel.ImageUrl);
    }
}