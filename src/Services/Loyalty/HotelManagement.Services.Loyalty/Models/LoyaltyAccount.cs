namespace HotelManagement.Services.Loyalty.Models;

public class LoyaltyAccount
{
    public Guid Id { get; set; }
    public Guid GuestId { get; set; }
    public int Points { get; set; }
    public LoyaltyTier Tier { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public enum LoyaltyTier
{
    Standard,
    Silver,
    Gold,
    Platinum,
    Diamond
}
