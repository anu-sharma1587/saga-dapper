using HotelManagement.Services.Guest.DTOs;

namespace HotelManagement.Services.Guest.Services;

public interface IGuestService
{
    Task<GuestProfileResponse> CreateGuestProfileAsync(CreateGuestProfileRequest request);
    Task<GuestProfileResponse> GetGuestProfileByIdAsync(Guid id);
    Task<GuestProfileResponse> GetGuestProfileByUserIdAsync(Guid userId);
    Task<GuestProfileResponse> UpdateGuestProfileAsync(Guid id, UpdateGuestProfileRequest request);
    Task<GuestProfileResponse> AddPreferenceAsync(Guid guestId, GuestPreferenceRequest preference);
    Task<bool> RemovePreferenceAsync(Guid guestId, Guid preferenceId);
    Task<GuestProfileResponse> UpdateLoyaltyPointsAsync(Guid id, UpdateLoyaltyPointsRequest request);
    Task<List<GuestProfileResponse>> SearchGuestsAsync(string searchTerm);
    Task<List<GuestProfileResponse>> GetGuestsByLoyaltyTierAsync(LoyaltyTier tier);
}
