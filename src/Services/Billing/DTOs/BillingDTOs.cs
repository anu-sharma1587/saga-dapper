namespace HotelManagement.Services.Billing.DTOs;

public record CreateInvoiceRequest(
    Guid ReservationId,
    Guid GuestId,
    decimal Amount,
    string Currency,
    string? Notes
);

public record InvoiceResponse
{
    public Guid Id { get; set; }
    public Guid ReservationId { get; set; }
    public Guid GuestId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime IssuedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public string Status { get; set; } = null!;
    public string? Notes { get; set; }
}
