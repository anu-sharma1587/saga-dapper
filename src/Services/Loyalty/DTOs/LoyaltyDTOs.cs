namespace HotelManagement.Services.Loyalty.DTOs;

public record AddPointsRequest(Guid GuestId, int Points, string Reason);
public record RedeemPointsRequest(Guid GuestId, int Points, string Reason);
public record LoyaltyAccountResponse
{
    public Guid Id { get; set; }
    public Guid GuestId { get; set; }
    public int Points { get; set; }
    public string Tier { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
