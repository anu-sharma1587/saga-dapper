using HotelManagement.Services.Loyalty.DTOs;

namespace HotelManagement.Services.Loyalty.Services;

public interface ILoyaltyService
{
    Task<LoyaltyAccountResponse> AddPointsAsync(AddPointsRequest request);
    Task<LoyaltyAccountResponse> RedeemPointsAsync(RedeemPointsRequest request);
    Task<LoyaltyAccountResponse?> GetAccountByGuestIdAsync(Guid guestId);
    Task<bool> CompensateRevertPointsAsync(Guid guestId, int points, string reason);
}
