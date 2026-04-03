using HotelManagement.Domain.Entities.Abstractions;

namespace HotelManagement.Domain.Entities;

public class Department : IEntity
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public long HotelId { get; set; }
    public long? ManagerId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public virtual Hotel Hotel { get; set; } = null!;
    public virtual Employee? Manager { get; set; }
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    public ICollection<HotelTaskType> TaskTypes { get; set; } = new List<HotelTaskType>();
}