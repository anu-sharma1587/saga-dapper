using DataAccess.Dapper;
using HotelManagement.Services.Payment.Models;

namespace HotelManagement.Services.Payment.SpInput;

public class ProcessPaymentParams : IStoredProcedureParams
{
    public string StoredProcedureName => "sp_process_payment";
    public object? p_refcur_1 { get; set; }
    
    public Guid Id { get; set; }
    public Guid ReservationId { get; set; }
    public Guid GuestId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public PaymentStatus Status { get; set; }
    public string Method { get; set; } = string.Empty;
    public string? TransactionId { get; set; }
    public string? PaymentIntentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ProcessedAt { get; set; }
    public string? FailureReason { get; set; }
    public string? ReceiptUrl { get; set; }
    public string Type { get; set; } = string.Empty;
    public bool IsRefunded { get; set; }
    public decimal? RefundedAmount { get; set; }
    public string? RefundTransactionId { get; set; }
    public DateTime? RefundedAt { get; set; }
    public string? RefundReason { get; set; }
}
