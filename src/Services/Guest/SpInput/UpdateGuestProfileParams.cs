using DataAccess;

namespace HotelManagement.Services.Guest.SpInput;

public class UpdateGuestProfileParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_update_guest_profile";
    public object? p_refcur_1 { get; set; }
    
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
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
    public bool? NewsletterSubscribed { get; set; }
    public DateTime ModifiedAt { get; set; }
}
