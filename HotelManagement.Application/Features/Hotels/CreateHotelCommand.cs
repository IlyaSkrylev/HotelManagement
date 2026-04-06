using HotelManagement.Application.Abstractions;
using HotelManagement.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Application.Features.Hotels;

public record CreateHotelCommand(
    string Name,
    string Address,
    string Phone,
    string Email,
    string Description
) : IRequest<CreateHotelResponse>;

public record CreateHotelResponse(long Id, string Name);

public class CreateHotelCommandHandler : IRequestHandler<CreateHotelCommand, CreateHotelResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CreateHotelCommandHandler> _logger;
    private readonly ICurrentUserService _currentUserService;

    public CreateHotelCommandHandler(
        IApplicationDbContext context,
        ILogger<CreateHotelCommandHandler> logger,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<CreateHotelResponse> Handle(CreateHotelCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Создание новой гостиницы: {Name}", request.Name);

        var hotel = new Hotel
        {
            Name = request.Name,
            Address = request.Address,
            Phone = request.Phone,
            Email = request.Email,
            Description = request.Description,
            CreatedById = _currentUserService.UserId,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Гостиница создана с ID: {HotelId}", hotel.Id);

        return new CreateHotelResponse(hotel.Id, hotel.Name);
    }
}