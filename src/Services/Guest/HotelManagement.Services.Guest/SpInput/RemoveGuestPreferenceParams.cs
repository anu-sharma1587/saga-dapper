using DataAccess;

namespace HotelManagement.Services.Guest.SpInput;

public class RemoveGuestPreferenceParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_remove_guest_preference";
    public object? p_refcur_1 { get; set; }
    
    public Guid GuestId { get; set; }
    public Guid PreferenceId { get; set; }
}
