using DataAccess;

namespace HotelManagement.Services.Guest.SpInput;

public class CreateGuestProfileParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_create_guest_profile";
    public object? p_refcur_1 { get; set; }
    
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
    public DateTime CreatedAt { get; set; }
    public string LoyaltyTier { get; set; } = null!;
    public int LoyaltyPoints { get; set; }
}
