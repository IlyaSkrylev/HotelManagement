using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelManagement.Domain.Entities;

namespace HotelManagement.Infrastructure.Data.MappingConfigurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("employees");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.DepartmentId)
            .HasColumnName("department_id")
            .IsRequired();

        builder.Property(x => x.Position)
            .HasColumnName("position")
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.HireDate)
            .HasColumnName("hire_date")
            .IsRequired();

        builder.Property(x => x.DismissalDate)
            .HasColumnName("dismissal_date");

        builder.Property(x => x.DismissalReason)
            .HasColumnName("dismissal_reason")
            .HasMaxLength(10000);

        builder.Property(x => x.IsActive)
            .HasColumnName("is_active");

        builder.Property(x => x.Salary)
            .HasColumnName("salary");

        builder.Property(x => x.SalarySupplement)
            .HasColumnName("salary_supplement");

        builder.Property(x => x.EmploymentTypeId)
            .HasColumnName("employment_type_id")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(x => x.ShiftCycleStartDate)
            .HasColumnName("shift_cycle_start_date")
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(u => u.Employees)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Department)
            .WithMany(d => d.Employees)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.EmploymentType)
            .WithMany()
            .HasForeignKey(x => x.EmploymentTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}