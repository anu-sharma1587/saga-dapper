using DataAccess.Dapper;
using DataAccess.DbConnectionProvider;
using HotelManagement.Services.Payment.DTOs;
using HotelManagement.Services.Payment.Models;
using HotelManagement.Services.Payment.SpInput;
using Microsoft.Extensions.Logging;

namespace HotelManagement.Services.Payment.Services;

public class PaymentService : IPaymentService
{
    private readonly IDapperDataRepository _dataRepository;
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(IDapperDataRepository dataRepository, IDbConnectionFactory connectionFactory, ILogger<PaymentService> logger)
    {
        _dataRepository = dataRepository;
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request)
    {
        try
        {
            using var connection = await _connectionFactory.CreateAsync();
            
            var payment = new Models.Payment
            {
                Id = Guid.NewGuid(),
                ReservationId = request.ReservationId,
                GuestId = request.GuestId,
                Amount = request.Amount,
                Currency = request.Currency,
                Method = request.PaymentMethod,
                Status = PaymentStatus.Completed, // Simulate success
                Type = request.PaymentType,
                CreatedAt = DateTime.UtcNow,
                ProcessedAt = DateTime.UtcNow,
                IsRefunded = false
            };

            await _dataRepository.AddAsync(payment, connection);

            return new PaymentResult
            {
                Success = true,
                PaymentId = payment.Id,
                TransactionId = payment.TransactionId ?? Guid.NewGuid().ToString()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment for reservation {ReservationId}", request.ReservationId);
            return new PaymentResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<RefundResult> RefundPaymentAsync(RefundRequest request)
    {
        try
        {
            using var connection = await _connectionFactory.CreateAsync();
            
            // First retrieve the payment
            var payment = await _dataRepository.FindByIDAsync<Models.Payment>(request.PaymentId, connection);
            if (payment == null)
            {
                return new RefundResult { Success = false, ErrorMessage = "Payment not found" };
            }

            // Create a refund record
            var refundId = Guid.NewGuid();
            var refund = new RefundRecord
            {
                Id = refundId,
                PaymentId = payment.Id,
                Amount = request.Amount,
                Reason = request.Reason,
                Status = RefundStatus.Completed,
                CreatedAt = DateTime.UtcNow,
                ProcessedAt = DateTime.UtcNow,
                TransactionId = Guid.NewGuid().ToString()
            };

            await _dataRepository.AddAsync(refund, connection);

            // Update the payment status
            payment.Status = PaymentStatus.Refunded;
            payment.IsRefunded = true;
            payment.RefundedAmount = request.Amount;
            payment.RefundTransactionId = refund.TransactionId;
            payment.RefundedAt = DateTime.UtcNow;
            payment.RefundReason = request.Reason;

            await _dataRepository.UpdateAsync(payment, payment.Id, connection);

            return new RefundResult
            {
                Success = true,
                RefundId = refundId,
                TransactionId = refund.TransactionId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refunding payment {PaymentId}", request.PaymentId);
            return new RefundResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<Models.Payment?> GetPaymentByIdAsync(Guid paymentId)
    {
        try
        {
            using var connection = await _connectionFactory.CreateAsync();
            return await _dataRepository.FindByIDAsync<Models.Payment>(paymentId, connection);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payment with ID {PaymentId}", paymentId);
            throw;
        }
    }

    public async Task<IEnumerable<Models.Payment>> GetPaymentsByReservationIdAsync(Guid reservationId)
    {
        try
        {
            using var connection = await _connectionFactory.CreateAsync();
            
            // Note: This would need a custom method or we'd need to implement a more specific query method
            // For now, returning empty as the current interface doesn't support this specific query
            _logger.LogWarning("GetPaymentsByReservationIdAsync not fully implemented with current interface");
            return Enumerable.Empty<Models.Payment>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payments for reservation {ReservationId}", reservationId);
            throw;
        }
    }
}
