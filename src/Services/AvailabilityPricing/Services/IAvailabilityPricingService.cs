using HotelManagement.Services.AvailabilityPricing.DTOs;

namespace HotelManagement.Services.AvailabilityPricing.Services;

public interface IAvailabilityPricingService
{
    Task<AvailabilityPricingResponse> CreateAvailabilityPricingAsync(CreateAvailabilityPricingRequest request);
    Task<AvailabilityPricingResponse?> GetAvailabilityPricingByIdAsync(Guid id);
    Task<IEnumerable<AvailabilityPricingResponse>> GetAvailabilityPricingsByHotelAsync(Guid hotelId);
    Task<bool> CompensateCancelAvailabilityPricingAsync(Guid id);
}
