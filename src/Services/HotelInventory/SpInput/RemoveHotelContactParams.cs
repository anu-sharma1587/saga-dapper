using DataAccess;

namespace HotelManagement.Services.HotelInventory.SpInput;

public class RemoveHotelContactParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_remove_hotel_contact";
    public object? p_refcur_1 { get; set; }
    
    public Guid Id { get; set; }
}
