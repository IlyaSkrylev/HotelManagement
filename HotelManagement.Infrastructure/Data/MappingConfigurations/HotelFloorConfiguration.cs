using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelManagement.Domain.Entities;

namespace HotelManagement.Infrastructure.Data.MappingConfigurations;

public class HotelFloorConfiguration : IEntityTypeConfiguration<HotelFloor>
{
    public void Configure(EntityTypeBuilder<HotelFloor> builder)
    {
        builder.ToTable("hotel_floors");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.HotelId)
            .HasColumnName("hotel_id")
            .IsRequired();

        builder.Property(x => x.FloorNumber)
            .HasColumnName("floor_number")
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(10000);

        builder.HasOne(x => x.Hotel)
            .WithMany(h => h.HotelFloors)
            .HasForeignKey(x => x.HotelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}