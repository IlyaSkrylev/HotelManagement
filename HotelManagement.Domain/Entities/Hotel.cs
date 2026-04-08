using HotelManagement.Domain.Entities.Abstractions;

namespace HotelManagement.Domain.Entities;

public class Hotel : IEntity
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public long CreatedById { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public virtual User CreatedBy { get; set; } = null!;
    public ICollection<Department> Departments { get; set; } = new List<Department>();
    public ICollection<HotelFloor> HotelFloors { get; set; } = new List<HotelFloor>();
    public ICollection<HotelRoom> HotelRooms { get; set; } = new List<HotelRoom>();
    public ICollection<UserHotelRole> UserHotelRoles { get; set; } = new List<UserHotelRole>();
    public ICollection<Resume> Resumes { get; set; } = new List<Resume>();
}