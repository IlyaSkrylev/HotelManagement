using HotelManagement.Domain.Entities.Abstractions;

namespace HotelManagement.Domain.Entities;

public class UserHotelRole : IEntity
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long HotelId { get; set; }
    public long RoleId { get; set; }
    public DateTimeOffset AssignedAt { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual Hotel Hotel { get; set; } = null!;
    public virtual UserRole Role { get; set; } = null!;
}