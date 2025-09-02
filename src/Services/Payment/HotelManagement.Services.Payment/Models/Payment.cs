using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Services.Payment.Models;

public class Payment
{
    public Guid Id { get; set; }
    public Guid ReservationId { get; set; }
    public Guid GuestId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public PaymentStatus Status { get; set; }
    public PaymentMethod Method { get; set; }
    public string? TransactionId { get; set; }
    public string? PaymentIntentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? FailureReason { get; set; }
    public string? ReceiptUrl { get; set; }
    public PaymentType Type { get; set; }
    public bool IsRefunded { get; set; }
    public decimal? RefundedAmount { get; set; }
    public string? RefundTransactionId { get; set; }
    public DateTime? RefundedAt { get; set; }
    public string? RefundReason { get; set; }
}

public enum PaymentStatus
{
    Pending,
    Processing,
    Completed,
    Failed,
    Refunded,
    PartiallyRefunded,
    Cancelled
}

public enum PaymentMethod
{
    CreditCard,
    DebitCard,
    BankTransfer,
    PayPal,
    Cash,
    CryptoWallet,
    Other
}

public enum PaymentType
{
    Deposit,
    FullPayment,
    ExtraCharges,
    Refund
}

public class PaymentCard
{
    public string Number { get; set; } = null!;
    public string ExpiryMonth { get; set; } = null!;
    public string ExpiryYear { get; set; } = null!;
    public string Cvv { get; set; } = null!;
    public string CardholderName { get; set; } = null!;
}
