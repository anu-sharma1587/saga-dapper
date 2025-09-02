using DataAccess;

namespace HotelManagement.Services.AvailabilityPricing.SpInput;

public class GetAvailabilityPricingByIdParams : IStoredProcedureParams
{
    public Guid Id { get; set; }

    public string StoredProcedureName => "sp_GetAvailabilityPricingById";
}
