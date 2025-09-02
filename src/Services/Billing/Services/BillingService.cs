using HotelManagement.Services.Billing.DTOs;
using HotelManagement.Services.Billing.Models;
using HotelManagement.Services.Billing.SpInput;
using DataAccess;

namespace HotelManagement.Services.Billing.Services;

public class BillingService : IBillingService
{
    private readonly IDataRepository _dataRepository;
    private readonly ILogger<BillingService> _logger;

    public BillingService(IDataRepository dataRepository, ILogger<BillingService> logger)
    {
        _dataRepository = dataRepository;
        _logger = logger;
    }

    public async Task<InvoiceResponse> CreateInvoiceAsync(CreateInvoiceRequest request)
    {
        try
        {
            var parameters = new CreateInvoiceParams
            {
                Id = Guid.NewGuid(),
                ReservationId = request.ReservationId,
                GuestId = request.GuestId,
                Amount = request.Amount,
                Currency = request.Currency,
                IssuedAt = DateTime.UtcNow,
                Status = "Pending",
                Notes = request.Notes
            };

            var result = await _dataRepository.ExecuteStoredProcedureAsync<Invoice, CreateInvoiceParams>(parameters);
            
            return MapToResponse(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating invoice");
            throw;
        }
    }

    public async Task<InvoiceResponse?> GetInvoiceByIdAsync(Guid id)
    {
        try
        {
            var parameters = new GetInvoiceByIdParams { Id = id };
            var result = await _dataRepository.QueryFirstOrDefaultStoredProcedureAsync<Invoice, GetInvoiceByIdParams>(parameters);
            
            return result == null ? null : MapToResponse(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting invoice by id {Id}", id);
            return null;
        }
    }

    public async Task<IEnumerable<InvoiceResponse>> GetInvoicesByGuestIdAsync(Guid guestId)
    {
        try
        {
            var parameters = new GetInvoicesByGuestIdParams { GuestId = guestId };
            var results = await _dataRepository.QueryStoredProcedureAsync<Invoice, GetInvoicesByGuestIdParams>(parameters);
            
            return results.Select(MapToResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting invoices by guest id {GuestId}", guestId);
            return Enumerable.Empty<InvoiceResponse>();
        }
    }

    public async Task<bool> MarkInvoicePaidAsync(Guid id)
    {
        try
        {
            var parameters = new MarkInvoicePaidParams 
            { 
                Id = id,
                PaidAt = DateTime.UtcNow
            };
            var result = await _dataRepository.ExecuteStoredProcedureAsync<MarkInvoicePaidParams>(parameters);
            
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking invoice as paid {Id}", id);
            return false;
        }
    }

    private static InvoiceResponse MapToResponse(Invoice invoice) => new()
    {
        Id = invoice.Id,
        ReservationId = invoice.ReservationId,
        GuestId = invoice.GuestId,
        Amount = invoice.Amount,
        Currency = invoice.Currency,
        IssuedAt = invoice.IssuedAt,
        PaidAt = invoice.PaidAt,
        Status = invoice.Status.ToString(),
        Notes = invoice.Notes
    };
}
