using DataAccess;
using HotelManagement.Services.Payment.DTOs;
using HotelManagement.Services.Payment.Models;
using HotelManagement.Services.Payment.SpInput;

namespace HotelManagement.Services.Payment.Services;

public class PaymentService : IPaymentService
{
    private readonly IDataRepository _dataRepository;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(IDataRepository dataRepository, ILogger<PaymentService> logger)
    {
        _dataRepository = dataRepository;
        _logger = logger;
    }

    public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request)
    {
        try
        {
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

            var parameters = new ProcessPaymentParams
            {
                Id = payment.Id,
                ReservationId = payment.ReservationId,
                GuestId = payment.GuestId,
                Amount = payment.Amount,
                Currency = payment.Currency,
                Status = payment.Status,
                Method = payment.Method,
                TransactionId = payment.TransactionId,
                PaymentIntentId = payment.PaymentIntentId,
                CreatedAt = payment.CreatedAt,
                ProcessedAt = payment.ProcessedAt,
                FailureReason = payment.FailureReason,
                ReceiptUrl = payment.ReceiptUrl,
                Type = payment.Type,
                IsRefunded = payment.IsRefunded,
                RefundedAmount = payment.RefundedAmount,
                RefundTransactionId = payment.RefundTransactionId,
                RefundedAt = payment.RefundedAt,
                RefundReason = payment.RefundReason
            };

            await _dataRepository.ExecuteAsync(parameters);

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
            // First retrieve the payment
            var paymentParams = new GetPaymentByIdParams
            {
                Id = request.PaymentId
            };

            var payment = await _dataRepository.QueryFirstOrDefaultAsync<Models.Payment>(paymentParams);
            if (payment == null)
            {
                return new RefundResult { Success = false, ErrorMessage = "Payment not found" };
            }

            // Create a refund record
            var refundId = Guid.NewGuid();
            var refundParams = new RefundPaymentParams
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

            await _dataRepository.ExecuteAsync(refundParams);

            // Update the payment status
            var updateParams = new UpdatePaymentStatusParams
            {
                Id = payment.Id,
                Status = PaymentStatus.Refunded,
                IsRefunded = true,
                RefundedAmount = request.Amount,
                RefundTransactionId = refundParams.TransactionId,
                RefundedAt = DateTime.UtcNow,
                RefundReason = request.Reason
            };

            await _dataRepository.ExecuteAsync(updateParams);

            return new RefundResult
            {
                Success = true,
                RefundId = refundId,
                TransactionId = refundParams.TransactionId
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
            var parameters = new GetPaymentByIdParams
            {
                Id = paymentId
            };

            return await _dataRepository.QueryFirstOrDefaultAsync<Models.Payment>(parameters);
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
            var parameters = new GetPaymentsByReservationIdParams
            {
                ReservationId = reservationId
            };

            return await _dataRepository.QueryAsync<Models.Payment>(parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving payments for reservation {ReservationId}", reservationId);
            throw;
        }
    }
}
