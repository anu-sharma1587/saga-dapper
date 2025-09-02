using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Services.Guest.DTOs;

public record CreateGuestProfileRequest(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    string? Address,
    string? City,
    string? State,
    string? Country,
    string? PostalCode,
    DateTime? DateOfBirth,
    string? Nationality,
    string? PassportNumber,
    DateTime? PassportExpiryDate,
    string? PreferredLanguage,
    bool NewsletterSubscribed,
    List<GuestPreferenceRequest>? Preferences
);

public record UpdateGuestProfileRequest(
    string? FirstName,
    string? LastName,
    string? PhoneNumber,
    string? Address,
    string? City,
    string? State,
    string? Country,
    string? PostalCode,
    DateTime? DateOfBirth,
    string? Nationality,
    string? PassportNumber,
    DateTime? PassportExpiryDate,
    string? PreferredLanguage,
    bool? NewsletterSubscribed
);

public record GuestPreferenceRequest(
    PreferenceType Type,
    string Value,
    string? Notes
);

public record UpdateLoyaltyPointsRequest(
    int PointsToAdd,
    string Reason
);

public record GuestProfileResponse
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string? PhoneNumber { get; init; }
    public string? Address { get; init; }
    public string? City { get; init; }
    public string? State { get; init; }
    public string? Country { get; init; }
    public string? PostalCode { get; init; }
    public DateTime? DateOfBirth { get; init; }
    public string? Nationality { get; init; }
    public string? PassportNumber { get; init; }
    public DateTime? PassportExpiryDate { get; init; }
    public string? PreferredLanguage { get; init; }
    public bool NewsletterSubscribed { get; init; }
    public List<GuestPreferenceResponse> Preferences { get; init; } = new();
    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; init; }
    public LoyaltyTier LoyaltyTier { get; init; }
    public int LoyaltyPoints { get; init; }
}

public record GuestPreferenceResponse
{
    public Guid Id { get; init; }
    public PreferenceType Type { get; init; }
    public string Value { get; init; } = null!;
    public string? Notes { get; init; }
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
