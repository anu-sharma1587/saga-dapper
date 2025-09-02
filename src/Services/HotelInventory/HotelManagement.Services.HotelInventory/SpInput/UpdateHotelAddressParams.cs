using DataAccess;

namespace HotelManagement.Services.HotelInventory.SpInput;

public class UpdateHotelAddressParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_update_hotel_address";
    public object? p_refcur_1 { get; set; }
    
    public Guid HotelId { get; set; }
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime UpdatedAt { get; set; }
}
