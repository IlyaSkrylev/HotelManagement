using HotelManagement.Application.Abstractions;
using HotelManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Infrastructure.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<UserHotelRole> UserHotelRoles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<EmploymentType> EmploymentTypes { get; set; }
    public DbSet<WorkShift> WorkShifts { get; set; }
    public DbSet<ShiftType> ShiftTypes { get; set; }
    public DbSet<HotelFloor> HotelFloors { get; set; }
    public DbSet<HotelRoom> HotelRooms { get; set; }
    public DbSet<RoomStatus> RoomStatuses { get; set; }
    public DbSet<HotelTask> Tasks { get; set; }
    public DbSet<HotelTaskType> TaskTypes { get; set; }
    public DbSet<HotelTaskStatus> TaskStatuses { get; set; }
    public DbSet<HotelTaskPriority> TaskPriorities { get; set; }
    public DbSet<FinancialOperation> FinancialOperations { get; set; }
    public DbSet<Resume> Resumes { get; set; }
    public DbSet<ResumeStatus> ResumeStatuses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}