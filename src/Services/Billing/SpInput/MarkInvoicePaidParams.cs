using DataAccess;

namespace HotelManagement.Services.Billing.SpInput;

public class MarkInvoicePaidParams : IStoredProcedureParams
{
    public Guid Id { get; set; }
    public DateTime PaidAt { get; set; }

    public string StoredProcedureName => "sp_MarkInvoicePaid";
}
