using DataAccess;

namespace HotelManagement.Services.HotelInventory.SpInput;

public class UpdateRoomInventoryParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_update_room_inventory";
    public object? p_refcur_1 { get; set; }
    
    public Guid HotelId { get; set; }
    public Guid RoomTypeId { get; set; }
    public int NewTotalRooms { get; set; }
    public DateTime UpdatedAt { get; set; }
}
