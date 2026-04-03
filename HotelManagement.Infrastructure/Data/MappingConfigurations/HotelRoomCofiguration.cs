using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelManagement.Domain.Entities;

namespace HotelManagement.Infrastructure.Data.MappingConfigurations;

public class HotelRoomConfiguration : IEntityTypeConfiguration<HotelRoom>
{
    public void Configure(EntityTypeBuilder<HotelRoom> builder)
    {
        builder.ToTable("hotel_rooms");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.HotelId)
            .HasColumnName("hotel_id")
            .IsRequired();

        builder.Property(x => x.FloorId)
            .HasColumnName("floor_id")
            .IsRequired();

        builder.Property(x => x.RoomNumber)
            .HasColumnName("room_number")
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.RoomStatusId)
            .HasColumnName("room_status_id")
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(10000);

        builder.HasOne(x => x.Hotel)
            .WithMany(h => h.HotelRooms)
            .HasForeignKey(x => x.HotelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Floor)
            .WithMany(f => f.HotelRooms)
            .HasForeignKey(x => x.FloorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.RoomStatus)
            .WithMany()
            .HasForeignKey(x => x.RoomStatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}