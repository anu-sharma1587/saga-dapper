using HotelManagement.Services.CheckInOut.DTOs;

namespace HotelManagement.Services.CheckInOut.Services;

public interface ICheckInOutService
{
    Task<CheckInOutResponse> CheckInAsync(CheckInRequest request);
    Task<CheckInOutResponse> CheckOutAsync(CheckOutRequest request);
    Task<CheckInOutResponse?> GetByReservationIdAsync(Guid reservationId);
    Task<bool> CompensateCancelAsync(Guid reservationId);
}
