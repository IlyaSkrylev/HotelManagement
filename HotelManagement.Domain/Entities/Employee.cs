using HotelManagement.Domain.Entities.Abstractions;

namespace HotelManagement.Domain.Entities;

public class Employee : IEntity
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long DepartmentId { get; set; }
    public required string Position { get; set; }
    public DateTimeOffset HireDate { get; set; }
    public DateTimeOffset? DismissalDate { get; set; }
    public string? DismissalReason { get; set; }
    public bool IsActive { get; set; } = true;
    public int? Salary { get; set; }
    public int? SalarySupplement { get; set; }
    public long EmploymentTypeId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset ShiftCycleStartDate { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual Department Department { get; set; } = null!;
    public virtual EmploymentType EmploymentType { get; set; } = null!;
    public ICollection<WorkShift> WorkShifts { get; set; } = new List<WorkShift>();
    public ICollection<HotelTask> AssignedTasks { get; set; } = new List<HotelTask>();
    public ICollection<HotelTask> CreatedTasks { get; set; } = new List<HotelTask>();
    public ICollection<FinancialOperation> FinancialOperations { get; set; } = new List<FinancialOperation>();
    public ICollection<FinancialOperation> CreatedFinancialOperations { get; set; } = new List<FinancialOperation>();
    public ICollection<Resume> ReviewedResumes { get; set; } = new List<Resume>();
}