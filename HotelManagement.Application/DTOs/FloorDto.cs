namespace HotelManagement.Application.DTOs;

public class FloorDto
{
    public long Id { get; set; }
    public long HotelId { get; set; }
    public int FloorNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int RoomsCount { get; set; }
}

public class CreateFloorDto
{
    public int FloorNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateFloorDto
{
    public int FloorNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}