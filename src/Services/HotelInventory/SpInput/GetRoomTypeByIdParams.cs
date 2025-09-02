using DataAccess;

namespace HotelManagement.Services.HotelInventory.SpInput;

public class GetRoomTypeByIdParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_get_room_type_by_id";
    public object? p_refcur_1 { get; set; }
    
    public Guid Id { get; set; }
}
