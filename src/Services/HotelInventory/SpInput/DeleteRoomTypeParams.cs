using DataAccess;

namespace HotelManagement.Services.HotelInventory.SpInput;

public class DeleteRoomTypeParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_delete_room_type";
    public object? p_refcur_1 { get; set; }
    
    public Guid Id { get; set; }
}
