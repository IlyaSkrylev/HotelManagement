using HotelManagement.Domain.Entities.Abstractions;

namespace HotelManagement.Domain.Entities;

public class FinancialOperation : IEntity
{
    public long Id { get; set; }
    public long EmployeeId { get; set; }
    public int Amount { get; set; }
    public string? Description { get; set; }
    public long CreatedById { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public virtual Employee Employee { get; set; } = null!;
    public virtual Employee CreatedBy { get; set; } = null!;
}