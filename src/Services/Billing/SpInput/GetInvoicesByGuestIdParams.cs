using DataAccess.Dapper;

namespace HotelManagement.Services.Billing.SpInput;

public class GetInvoicesByGuestIdParams : IStoredProcedureParams
{
    public Guid GuestId { get; set; }
}
