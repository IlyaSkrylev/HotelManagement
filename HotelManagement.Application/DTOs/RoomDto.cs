namespace HotelManagement.Application.DTOs;

public class RoomDto
{
    public long Id { get; set; }
    public long HotelId { get; set; }
    public long FloorId { get; set; }
    public string FloorName { get; set; } = string.Empty;
    public int FloorNumber { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public long RoomStatusId { get; set; }
    public string RoomStatusName { get; set; } = string.Empty;
    public string? RoomStatusColor { get; set; }
    public string? Description { get; set; }
}

public class CreateRoomDto
{
    public long FloorId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public long RoomStatusId { get; set; }
    public string? Description { get; set; }
}

public class UpdateRoomDto
{
    public long FloorId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public long RoomStatusId { get; set; }
    public string? Description { get; set; }
}