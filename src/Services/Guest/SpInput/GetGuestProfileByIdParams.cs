using DataAccess;

namespace HotelManagement.Services.Guest.SpInput;

public class GetGuestProfileByIdParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_get_guest_profile_by_id";
    public object? p_refcur_1 { get; set; }
    
    public Guid Id { get; set; }
}
