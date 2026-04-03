using HotelManagement.Domain.Entities.Abstractions;

namespace HotelManagement.Domain.Entities;

public class HotelTaskType : IEntity
{
    public long Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public long DepartmentId { get; set; }

    public virtual Department Department { get; set; } = null!;
}