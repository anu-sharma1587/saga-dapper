using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Services.Guest.Models;

public class GuestProfile
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Nationality { get; set; }
    public string? PassportNumber { get; set; }
    public DateTime? PassportExpiryDate { get; set; }
    public string? PreferredLanguage { get; set; }
    public bool NewsletterSubscribed { get; set; }
    public List<GuestPreference> Preferences { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public LoyaltyTier LoyaltyTier { get; set; }
    public int LoyaltyPoints { get; set; }
}

public class GuestPreference
{
    public Guid Id { get; set; }
    public Guid GuestId { get; set; }
    public PreferenceType Type { get; set; }
    public string Value { get; set; } = null!;
    public string? Notes { get; set; }
}

public enum PreferenceType
{
    RoomType,
    BedType,
    FloorPreference,
    DietaryRequirement,
    Amenity,
    SpecialAssistance,
    Other
}

public enum LoyaltyTier
{
    Standard,
    Silver,
    Gold,
    Platinum,
    Diamond
}
