using DataAccess;

namespace HotelManagement.Services.Billing.SpInput;

public class GetInvoicesByGuestIdParams : IStoredProcedureParams
{
    public Guid GuestId { get; set; }

    public string StoredProcedureName => "sp_GetInvoicesByGuestId";
}
