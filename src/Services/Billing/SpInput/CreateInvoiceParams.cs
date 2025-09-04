using DataAccess.Dapper;

namespace HotelManagement.Services.Billing.SpInput;

public class CreateInvoiceParams : IStoredProcedureParams
{
    public Guid Id { get; set; }
    public Guid ReservationId { get; set; }
    public Guid GuestId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime IssuedAt { get; set; }
    public string Status { get; set; } = "Pending";
    public string? Notes { get; set; }

    public string StoredProcedureName => "sp_CreateInvoice";
}
