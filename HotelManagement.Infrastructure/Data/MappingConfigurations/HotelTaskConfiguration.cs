using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelManagement.Domain.Entities;

namespace HotelManagement.Infrastructure.Data.MappingConfigurations;

public class HotelTaskConfiguration : IEntityTypeConfiguration<HotelTask>
{
    public void Configure(EntityTypeBuilder<HotelTask> builder)
    {
        builder.ToTable("tasks");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.TaskTypeId)
            .HasColumnName("task_type_id")
            .IsRequired();

        builder.Property(x => x.TaskStatusId)
            .HasColumnName("task_status_id")
            .IsRequired();

        builder.Property(x => x.PriorityId)
            .HasColumnName("priority_id")
            .IsRequired();

        builder.Property(x => x.AssignedToId)
            .HasColumnName("assigned_to_id")
            .IsRequired();

        builder.Property(x => x.CreatedById)
            .HasColumnName("created_by_id")
            .IsRequired();

        builder.Property(x => x.RoomId)
            .HasColumnName("room_id");

        builder.Property(x => x.DueDate)
            .HasColumnName("due_date");

        builder.Property(x => x.CompletedAt)
            .HasColumnName("completed_at");

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasColumnName("notes")
            .HasMaxLength(10000);

        builder.HasOne(x => x.TaskType)
            .WithMany()
            .HasForeignKey(x => x.TaskTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.TaskStatus)
            .WithMany()
            .HasForeignKey(x => x.TaskStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Priority)
            .WithMany()
            .HasForeignKey(x => x.PriorityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AssignedTo)
            .WithMany(e => e.AssignedTasks)
            .HasForeignKey(x => x.AssignedToId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedBy)
            .WithMany(e => e.CreatedTasks)
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Room)
            .WithMany(r => r.Tasks)
            .HasForeignKey(x => x.RoomId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}