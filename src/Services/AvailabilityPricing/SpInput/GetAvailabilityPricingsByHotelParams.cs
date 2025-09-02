using DataAccess;

namespace HotelManagement.Services.AvailabilityPricing.SpInput;

public class GetAvailabilityPricingsByHotelParams : IStoredProcedureParams
{
    public Guid HotelId { get; set; }

    public string StoredProcedureName => "sp_GetAvailabilityPricingsByHotel";
}
