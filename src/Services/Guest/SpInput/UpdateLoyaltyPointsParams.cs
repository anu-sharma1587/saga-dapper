using DataAccess;

namespace HotelManagement.Services.Guest.SpInput;

public class UpdateLoyaltyPointsParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_update_loyalty_points";
    public object? p_refcur_1 { get; set; }
    
    public Guid Id { get; set; }
    public int PointsToAdd { get; set; }
    public string LoyaltyTier { get; set; } = null!;
    public DateTime ModifiedAt { get; set; }
}
