using HotelManagement.Domain.Entities.Abstractions;

namespace HotelManagement.Domain.Entities;

public class HotelFloor : IEntity
{
    public long Id { get; set; }
    public long HotelId { get; set; }
    public int FloorNumber { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }

    public virtual Hotel Hotel { get; set; } = null!;
    public ICollection<HotelRoom> HotelRooms { get; set; } = new List<HotelRoom>();
}