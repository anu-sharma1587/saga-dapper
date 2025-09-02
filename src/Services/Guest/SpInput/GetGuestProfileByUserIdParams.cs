using DataAccess;

namespace HotelManagement.Services.Guest.SpInput;

public class GetGuestProfileByUserIdParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_get_guest_profile_by_user_id";
    public object? p_refcur_1 { get; set; }
    
    public Guid UserId { get; set; }
}
