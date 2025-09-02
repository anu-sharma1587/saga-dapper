using HotelManagement.Services.Reservation.DTOs;

namespace HotelManagement.Services.Reservation.Services;

public interface IReservationService
{
    Task<ReservationResponse> CreateReservationAsync(CreateReservationRequest request);
    Task<ReservationResponse> GetReservationByIdAsync(Guid id);
    Task<IEnumerable<ReservationResponse>> GetReservationsByGuestIdAsync(Guid guestId);
    Task<IEnumerable<ReservationResponse>> GetReservationsByHotelIdAsync(Guid hotelId, DateTime? fromDate = null, DateTime? toDate = null);
    Task<ReservationResponse> UpdateReservationStatusAsync(Guid id, UpdateReservationRequest request);
    Task<bool> CancelReservationAsync(Guid id, string reason);
    Task<bool> CheckInAsync(Guid id);
    Task<bool> CheckOutAsync(Guid id);
    Task<bool> MarkAsNoShowAsync(Guid id);
    Task<bool> UpdatePaymentStatusAsync(Guid id, bool isPaid, decimal? depositAmount = null);
}
