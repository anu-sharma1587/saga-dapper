using DataAccess;

namespace HotelManagement.Services.HotelInventory.SpInput;

public class UpdateHotelContactParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_update_hotel_contact";
    public object? p_refcur_1 { get; set; }
    
    public Guid Id { get; set; }
    public string ContactType { get; set; } = string.Empty;
    public string ContactValue { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
}
