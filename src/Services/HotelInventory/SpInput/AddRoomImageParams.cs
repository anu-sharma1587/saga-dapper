using DataAccess;

namespace HotelManagement.Services.HotelInventory.SpInput;

public class AddRoomImageParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_add_room_image";
    public object? p_refcur_1 { get; set; }
    
    public Guid Id { get; set; }
    public Guid RoomTypeId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public int DisplayOrder { get; set; }
}
