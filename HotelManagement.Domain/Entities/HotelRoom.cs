using HotelManagement.Domain.Entities.Abstractions;

namespace HotelManagement.Domain.Entities;

public class HotelRoom : IEntity
{
    public long Id { get; set; }
    public long HotelId { get; set; }
    public long FloorId { get; set; }
    public required string RoomNumber { get; set; }
    public long RoomStatusId { get; set; }
    public string? Description { get; set; }

    public virtual Hotel Hotel { get; set; } = null!;
    public virtual HotelFloor Floor { get; set; } = null!;
    public virtual RoomStatus RoomStatus { get; set; } = null!;
    public ICollection<HotelTask> Tasks { get; set; } = new List<HotelTask>();
}