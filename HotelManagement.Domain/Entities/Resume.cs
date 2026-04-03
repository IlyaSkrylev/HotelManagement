using HotelManagement.Domain.Entities.Abstractions;

namespace HotelManagement.Domain.Entities;

public class Resume : IEntity
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long HotelId { get; set; }
    public required string DesiredPosition { get; set; }
    public string? Experience { get; set; }
    public string? Education { get; set; }
    public required string FileUrl { get; set; }
    public long StatusId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ReviewedAt { get; set; }
    public long? ReviewedById { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual Hotel Hotel { get; set; } = null!;
    public virtual ResumeStatus Status { get; set; } = null!;
    public virtual Employee? ReviewedBy { get; set; }
}