using DataAccess;

namespace HotelManagement.Services.AvailabilityPricing.SpInput;

public class CompensateCancelAvailabilityPricingParams : IStoredProcedureParams
{
    public Guid Id { get; set; }

    public string StoredProcedureName => "sp_CompensateCancelAvailabilityPricing";
}
