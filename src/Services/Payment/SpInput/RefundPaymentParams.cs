using DataAccess;
using HotelManagement.Services.Payment.Models;

namespace HotelManagement.Services.Payment.SpInput;

public class RefundPaymentParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_refund_payment";
    public object? p_refcur_1 { get; set; }
    
    public Guid Id { get; set; }
    public Guid PaymentId { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public RefundStatus Status { get; set; }
    public string? TransactionId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
}
