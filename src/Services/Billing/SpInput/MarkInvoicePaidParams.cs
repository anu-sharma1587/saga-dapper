using DataAccess.Dapper;

namespace HotelManagement.Services.Billing.SpInput;

public class MarkInvoicePaidParams : IStoredProcedureParams
{
    public Guid Id { get; set; }
    public DateTime PaidAt { get; set; }
}
