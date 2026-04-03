using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelManagement.Domain.Entities;

namespace HotelManagement.Infrastructure.Data.MappingConfigurations;

public class WorkShiftConfiguration : IEntityTypeConfiguration<WorkShift>
{
    public void Configure(EntityTypeBuilder<WorkShift> builder)
    {
        builder.ToTable("work_shifts");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.EmployeeId)
            .HasColumnName("employee_id")
            .IsRequired();

        builder.Property(x => x.ShiftTypeId)
            .HasColumnName("shift_type_id")
            .IsRequired();

        builder.Property(x => x.ShiftDate)
            .HasColumnName("shift_date")
            .IsRequired();

        builder.Property(x => x.StartTime)
            .HasColumnName("start_time");

        builder.Property(x => x.EndTime)
            .HasColumnName("end_time");

        builder.HasOne(x => x.Employee)
            .WithMany(e => e.WorkShifts)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ShiftType)
            .WithMany()
            .HasForeignKey(x => x.ShiftTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}