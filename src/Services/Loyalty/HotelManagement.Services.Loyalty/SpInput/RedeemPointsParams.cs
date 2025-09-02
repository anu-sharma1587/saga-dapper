using DataAccess;

namespace HotelManagement.Services.Loyalty.SpInput;

public class RedeemPointsParams : IStoredProcedureParams
{
    public Guid GuestId { get; set; }
    public int Points { get; set; }
    public string Reason { get; set; } = null!;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public string StoredProcedureName => "sp_RedeemLoyaltyPoints";
}
