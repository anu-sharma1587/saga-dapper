using DataAccess;

namespace HotelManagement.Services.HotelInventory.SpInput;

public class AddRoomAmenityParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_add_room_amenity";
    public object? p_refcur_1 { get; set; }
    
    public Guid Id { get; set; }
    public Guid RoomTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public bool IsHighlight { get; set; }
}
