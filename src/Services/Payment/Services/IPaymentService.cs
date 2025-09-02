using HotelManagement.Services.Payment.DTOs;
using HotelManagement.Services.Payment.Models;

namespace HotelManagement.Services.Payment.Services;

public interface IPaymentService
{
    Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request);
    Task<RefundResult> RefundPaymentAsync(RefundRequest request);
    Task<Payment?> GetPaymentByIdAsync(Guid paymentId);
    Task<IEnumerable<Payment>> GetPaymentsByReservationIdAsync(Guid reservationId);
}
