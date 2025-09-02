using DataAccess;

namespace HotelManagement.Services.HotelInventory.SpInput;

public class CreateRoomTypeParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_create_room_type";
    public object? p_refcur_1 { get; set; }
    
    public Guid Id { get; set; }
    public Guid HotelId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public decimal BasePrice { get; set; }
    public int AvailableRooms { get; set; }
    public string[] Amenities { get; set; } = Array.Empty<string>();
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
