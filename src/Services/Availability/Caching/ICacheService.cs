namespace HotelManagement.Services.Availability.Caching;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
    Task RemoveAsync(string key);
    Task RemoveByPrefixAsync(string prefix);
}

public static class CacheKeys
{
    public static string RoomAvailability(Guid hotelId, DateTime date) => $"availability:{hotelId}:{date:yyyyMMdd}";
    public static string PricingRules(Guid hotelId) => $"pricingrules:{hotelId}";
    public static string SeasonalPeriods(Guid hotelId) => $"seasons:{hotelId}";
    public static string SpecialEvents(Guid hotelId) => $"events:{hotelId}";
    public static string InventoryBlocks(Guid hotelId) => $"blocks:{hotelId}";
    public static string DemandForecast(Guid hotelId, Guid roomTypeId, DateTime date) => 
        $"demand:{hotelId}:{roomTypeId}:{date:yyyyMMdd}";
}
