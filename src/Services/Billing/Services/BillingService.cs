using HotelManagement.Services.Billing.DTOs;
using HotelManagement.Services.Billing.Models;
using HotelManagement.Services.Billing.SpInput;
using DataAccess.Dapper;
using DataAccess.DbConnectionProvider;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Services.Billing.Services;

public class BillingService : IBillingService
{
    private readonly IDapperDataRepository _dataRepository;
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<BillingService> _logger;

    public BillingService(IDapperDataRepository dataRepository, IDbConnectionFactory connectionFactory, ILogger<BillingService> logger)
    {
        _dataRepository = dataRepository;
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<InvoiceResponse> CreateInvoiceAsync(CreateInvoiceRequest request)
    {
        try
        {
            using var connection = await _connectionFactory.CreateAsync();
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

            var result = await _dataRepository.AddAsync(parameters, connection);
            
            return MapToResponse(new Invoice
            {
                Id = parameters.Id,
                ReservationId = parameters.ReservationId,
                GuestId = parameters.GuestId,
                Amount = parameters.Amount,
                Currency = parameters.Currency,
                IssuedAt = parameters.IssuedAt,
                Status = InvoiceStatus.Pending,
                Notes = parameters.Notes
            });
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
            using var connection = await _connectionFactory.CreateAsync();
            var result = await _dataRepository.FindByIDAsync<Invoice>(id, connection);
            
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
            using var connection = await _connectionFactory.CreateAsync();
            // Note: This would need a custom method or we'd need to implement a more specific query method
            // For now, returning empty as the current interface doesn't support this specific query
            _logger.LogWarning("GetInvoicesByGuestIdAsync not fully implemented with current interface");
            return Enumerable.Empty<InvoiceResponse>();
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
            using var connection = await _connectionFactory.CreateAsync();
            var invoice = await _dataRepository.FindByIDAsync<Invoice>(id, connection);
            if (invoice == null) return false;
            
            invoice.PaidAt = DateTime.UtcNow;
            invoice.Status = InvoiceStatus.Paid;
            
            var result = await _dataRepository.UpdateAsync(invoice, id, connection);
            
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
