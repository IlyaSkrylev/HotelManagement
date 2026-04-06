using HotelManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Application.Abstractions;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Hotel> Hotels { get; }
    DbSet<UserHotelRole> UserHotelRoles { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<Department> Departments { get; }
    DbSet<Employee> Employees { get; }
    DbSet<EmploymentType> EmploymentTypes { get; }
    DbSet<WorkShift> WorkShifts { get; }
    DbSet<ShiftType> ShiftTypes { get; }
    DbSet<HotelFloor> HotelFloors { get; }
    DbSet<HotelRoom> HotelRooms { get; }
    DbSet<RoomStatus> RoomStatuses { get; }
    DbSet<HotelTask> Tasks { get; }
    DbSet<HotelTaskType> TaskTypes { get; }
    DbSet<HotelTaskStatus> TaskStatuses { get; }
    DbSet<HotelTaskPriority> TaskPriorities { get; }
    DbSet<FinancialOperation> FinancialOperations { get; }
    DbSet<Resume> Resumes { get; }
    DbSet<ResumeStatus> ResumeStatuses { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}