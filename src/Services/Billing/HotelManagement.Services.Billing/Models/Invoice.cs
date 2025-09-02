namespace HotelManagement.Services.Billing.Models;

public class Invoice
{
    public Guid Id { get; set; }
    public Guid ReservationId { get; set; }
    public Guid GuestId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime IssuedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public InvoiceStatus Status { get; set; }
    public string? Notes { get; set; }
}

public enum InvoiceStatus
{
    Pending,
    Paid,
    Cancelled
}
