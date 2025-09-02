using DataAccess;

namespace HotelManagement.Services.HotelInventory.SpInput;

public class AddHotelAmenityParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_add_hotel_amenity";
    public object? p_refcur_1 { get; set; }
    
    public Guid HotelId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
