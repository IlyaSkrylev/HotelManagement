using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelManagement.Domain.Entities;

namespace HotelManagement.Infrastructure.Data.MappingConfigurations;

public class ShiftTypeConfiguration : IEntityTypeConfiguration<ShiftType>
{
    public void Configure(EntityTypeBuilder<ShiftType> builder)
    {
        builder.ToTable("shift_types");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Code)
            .HasColumnName("code")
            .IsRequired()
            .HasMaxLength(50);
        builder.HasIndex(x => x.Code).IsUnique();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Color)
            .HasColumnName("color")
            .HasMaxLength(7);

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(10000);

        builder.Property(x => x.TotalCycleDays)
            .HasColumnName("total_cycle_days")
            .IsRequired();

        builder.Property(x => x.WorkingDayShifts)
            .HasColumnName("working_day_shifts")
            .IsRequired();

        builder.Property(x => x.WorkingNightShifts)
            .HasColumnName("working_night_shifts")
            .IsRequired();

        builder.Property(x => x.RestDays)
            .HasColumnName("rest_days")
            .IsRequired();

        builder.Property(x => x.DayShiftStartTime)
            .HasColumnName("day_shift_start_time")
            .HasColumnType("time without time zone")
            .IsRequired();

        builder.Property(x => x.DayShiftEndTime)
            .HasColumnName("day_shift_end_time")
            .HasColumnType("time without time zone")
            .IsRequired();

        builder.Property(x => x.NightShiftStartTime)
            .HasColumnName("night_shift_start_time")
            .HasColumnType("time without time zone")
            .IsRequired();

        builder.Property(x => x.NightShiftEndTime)
            .HasColumnName("night_shift_end_time")
            .HasColumnType("time without time zone")
            .IsRequired();
    }
}