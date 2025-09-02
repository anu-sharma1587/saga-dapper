using DataAccess;
using HotelManagement.Services.Payment.Models;

namespace HotelManagement.Services.Payment.SpInput;

public class UpdatePaymentStatusParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_update_payment_status";
    public object? p_refcur_1 { get; set; }
    
    public Guid Id { get; set; }
    public PaymentStatus Status { get; set; }
    public bool IsRefunded { get; set; }
    public decimal? RefundedAmount { get; set; }
    public string? RefundTransactionId { get; set; }
    public DateTime? RefundedAt { get; set; }
    public string? RefundReason { get; set; }
}
