using DataAccess;

namespace HotelManagement.Services.Guest.SpInput;

public class GetGuestsByLoyaltyTierParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_get_guests_by_loyalty_tier";
    public object? p_refcur_1 { get; set; }
    
    public string LoyaltyTier { get; set; } = null!;
}
