using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace HotelManagement.Services.Availability.Caching;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly DistributedCacheEntryOptions _defaultOptions;

    public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
        _defaultOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
            SlidingExpiration = TimeSpan.FromMinutes(10)
        };
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var value = await _cache.GetStringAsync(key);
            if (string.IsNullOrEmpty(value))
                return default;

            return JsonSerializer.Deserialize<T>(value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving value for key {Key} from cache", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        try
        {
            var options = expiration.HasValue
                ? new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration }
                : _defaultOptions;

            var serializedValue = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, serializedValue, options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting value for key {Key} in cache", key);
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            await _cache.RemoveAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing key {Key} from cache", key);
        }
    }

    public async Task RemoveByPrefixAsync(string prefix)
    {
        try
        {
            // Note: This is a simplified implementation.
            // In a production environment, you might want to use Redis SCAN command
            // to efficiently find and remove keys by prefix
            _logger.LogWarning("RemoveByPrefix operation might not be optimal in Redis. Consider using key patterns or tags.");
            await _cache.RemoveAsync(prefix + "*");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing keys with prefix {Prefix} from cache", prefix);
        }
    }
}
