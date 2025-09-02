using DataAccess;

namespace HotelManagement.Services.HotelInventory.SpInput;

public class GetHotelAddressParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_get_hotel_address";
    public object? p_refcur_1 { get; set; }
    
    public Guid HotelId { get; set; }
}
