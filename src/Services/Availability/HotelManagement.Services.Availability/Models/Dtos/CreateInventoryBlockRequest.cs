namespace HotelManagement.Services.Availability.Models.Dtos;

public class CreateInventoryBlockRequest
{
    public Guid HotelId { get; set; }
    public Guid? RoomTypeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int BlockedRooms { get; set; }
    public string? Reason { get; set; }
    public string? Reference { get; set; }
}

public class UpdateInventoryBlockRequest : CreateInventoryBlockRequest
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
}
