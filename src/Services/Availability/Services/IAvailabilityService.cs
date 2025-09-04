using HotelManagement.Services.Availability.Models;
using HotelManagement.Services.Availability.Models.Dtos;

namespace HotelManagement.Services.Availability.Services;

public interface IAvailabilityService
{
    Task<List<RoomAvailabilityResponse>> GetAvailabilityAsync(
        Guid hotelId,
        DateTime checkIn,
        DateTime checkOut,
        List<Guid>? roomTypeIds = null);

    Task<bool> UpdateAvailabilityAsync(
        Guid hotelId,
        Guid roomTypeId,
        DateTime date,
        int availableRooms,
        decimal? basePrice = null);

    Task<PricingRule> CreatePricingRuleAsync(PricingRule rule);
    Task<PricingRule> UpdatePricingRuleAsync(PricingRule rule);
    Task<bool> DeletePricingRuleAsync(Guid id);
    Task<List<PricingRule>> GetActivePricingRulesAsync(Guid hotelId, Guid? roomTypeId = null);

    Task<InventoryBlock> CreateInventoryBlockAsync(InventoryBlock block);
    Task<bool> UpdateInventoryBlockAsync(InventoryBlock block);
    Task<bool> DeleteInventoryBlockAsync(Guid id);
    Task<List<InventoryBlock>> GetActiveInventoryBlocksAsync(Guid hotelId, Guid? roomTypeId = null);

    Task<SeasonalPeriod> CreateSeasonalPeriodAsync(SeasonalPeriod period);
    Task<SeasonalPeriod> UpdateSeasonalPeriodAsync(SeasonalPeriod period);
    Task<bool> DeleteSeasonalPeriodAsync(Guid id);
    Task<List<SeasonalPeriod>> GetActiveSeasonalPeriodsAsync(Guid hotelId);

    Task<SpecialEvent> CreateSpecialEventAsync(SpecialEvent specialEvent);
    Task<SpecialEvent> UpdateSpecialEventAsync(SpecialEvent specialEvent);
    Task<bool> DeleteSpecialEventAsync(Guid id);
    Task<List<SpecialEvent>> GetActiveSpecialEventsAsync(Guid hotelId);

    Task<PricingAnalysisResponse> GetPricingAnalysisAsync(
        Guid hotelId,
        Guid roomTypeId,
        DateTime date);

    Task UpdateDemandForecastAsync(HotelManagement.Services.Availability.Models.Dtos.DemandForecast forecast);
    Task<List<HotelManagement.Services.Availability.Models.Dtos.DemandForecast>> GetDemandForecastsAsync(
        Guid hotelId,
        Guid roomTypeId,
        DateTime startDate,
        DateTime endDate);
}
