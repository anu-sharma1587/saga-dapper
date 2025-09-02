using DataAccess;

namespace HotelManagement.Services.HotelInventory.SpInput;

public class GetHotelByIdParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_get_hotel_by_id";
    public object? p_refcur_1 { get; set; }
    
    public Guid Id { get; set; }
}
