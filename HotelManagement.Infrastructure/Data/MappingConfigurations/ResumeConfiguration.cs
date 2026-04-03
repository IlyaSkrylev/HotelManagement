using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelManagement.Domain.Entities;

namespace HotelManagement.Infrastructure.Data.MappingConfigurations;

public class ResumeConfiguration : IEntityTypeConfiguration<Resume>
{
    public void Configure(EntityTypeBuilder<Resume> builder)
    {
        builder.ToTable("resumes");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.HotelId)
            .HasColumnName("hotel_id")
            .IsRequired();

        builder.Property(x => x.DesiredPosition)
            .HasColumnName("desired_position")
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.Experience)
            .HasColumnName("experience")
            .HasMaxLength(10000);

        builder.Property(x => x.Education)
            .HasColumnName("education")
            .HasMaxLength(10000);

        builder.Property(x => x.FileUrl)
            .HasColumnName("file_url")
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.StatusId)
            .HasColumnName("status_id")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.ReviewedAt)
            .HasColumnName("reviewed_at");

        builder.Property(x => x.ReviewedById)
            .HasColumnName("reviewed_by_id");

        builder.HasOne(x => x.User)
            .WithMany(u => u.Resumes)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Hotel)
            .WithMany(h => h.Resumes)
            .HasForeignKey(x => x.HotelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Status)
            .WithMany()
            .HasForeignKey(x => x.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ReviewedBy)
            .WithMany(e => e.ReviewedResumes)
            .HasForeignKey(x => x.ReviewedById)
            .OnDelete(DeleteBehavior.SetNull);
    }