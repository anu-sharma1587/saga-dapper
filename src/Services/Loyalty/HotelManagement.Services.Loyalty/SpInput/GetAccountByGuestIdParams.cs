using DataAccess;

namespace HotelManagement.Services.Loyalty.SpInput;

public class GetAccountByGuestIdParams : IStoredProcedureParams
{
    public Guid GuestId { get; set; }

    public string StoredProcedureName => "sp_GetLoyaltyAccountByGuestId";
}
