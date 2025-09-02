using DataAccess;

namespace HotelManagement.Services.HotelInventory.SpInput;

public class GetRoomTypesByHotelIdParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_get_room_types_by_hotel_id";
    public object? p_refcur_1 { get; set; }
    
    public Guid HotelId { get; set; }
}
