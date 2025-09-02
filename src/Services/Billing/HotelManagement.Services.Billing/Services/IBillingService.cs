using HotelManagement.Services.Billing.DTOs;

namespace HotelManagement.Services.Billing.Services;

public interface IBillingService
{
    Task<InvoiceResponse> CreateInvoiceAsync(CreateInvoiceRequest request);
    Task<InvoiceResponse?> GetInvoiceByIdAsync(Guid id);
    Task<IEnumerable<InvoiceResponse>> GetInvoicesByGuestIdAsync(Guid guestId);
    Task<bool> MarkInvoicePaidAsync(Guid id);
}
