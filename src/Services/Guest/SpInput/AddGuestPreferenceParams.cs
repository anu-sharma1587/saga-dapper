using DataAccess;
using HotelManagement.Services.Guest.DTOs;

namespace HotelManagement.Services.Guest.SpInput;

public class AddGuestPreferenceParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_add_guest_preference";
    public object? p_refcur_1 { get; set; }
    
    public Guid Id { get; set; }
    public Guid GuestId { get; set; }
    public string PreferenceType { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string? Notes { get; set; }
}
