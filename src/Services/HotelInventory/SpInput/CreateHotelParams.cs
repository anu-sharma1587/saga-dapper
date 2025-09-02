using DataAccess;

namespace HotelManagement.Services.HotelInventory.SpInput;

public class CreateHotelParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_create_hotel";
    public object? p_refcur_1 { get; set; }
    
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int StarRating { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Address fields
    public string StreetAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    
    // Contact fields
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Website { get; set; }
    public string? FacebookPage { get; set; }
    public string? TwitterHandle { get; set; }
    public string? InstagramHandle { get; set; }
}
