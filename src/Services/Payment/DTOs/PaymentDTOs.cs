using HotelManagement.Services.Payment.Models;

namespace HotelManagement.Services.Payment.DTOs;

public record PaymentRequest(
    Guid ReservationId,
    Guid GuestId,
    decimal Amount,
    string Currency,
    PaymentMethod PaymentMethod,
    PaymentType PaymentType
);

public record PaymentResult
{
    public bool Success { get; set; }
    public Guid PaymentId { get; set; }
    public string? TransactionId { get; set; }
    public string? ErrorMessage { get; set; }
}

public record RefundRequest(
    Guid PaymentId,
    decimal Amount,
    string Reason
);

public record RefundResult
{
    public bool Success { get; set; }
    public Guid RefundId { get; set; }
    public string? TransactionId { get; set; }
    public string? ErrorMessage { get; set; }
}
