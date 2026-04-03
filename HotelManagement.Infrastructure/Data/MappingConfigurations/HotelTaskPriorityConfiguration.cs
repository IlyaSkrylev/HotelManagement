using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelManagement.Domain.Entities;

namespace HotelManagement.Infrastructure.Data.MappingConfigurations;

public class TaskPriorityConfiguration : IEntityTypeConfiguration<HotelTaskPriority>
{
    public void Configure(EntityTypeBuilder<HotelTaskPriority> builder)
    {
        builder.ToTable("task_priorities");

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

        builder.Property(x => x.Level)
            .HasColumnName("level");

        builder.Property(x => x.Color)
            .HasColumnName("color")
            .HasMaxLength(7);
    }
}