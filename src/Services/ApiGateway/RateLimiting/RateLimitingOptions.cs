namespace HotelManagement.Services.ApiGateway.RateLimiting;

public class RateLimitingOptions
{
    public int GlobalRequestsPerSecond { get; set; } = 100;
    public int AuthRequestsPerMinute { get; set; } = 20;
    public int SearchRequestsPerMinute { get; set; } = 30;
    public int BookingRequestsPerMinute { get; set; } = 10;
    public int UserRequestsPerMinute { get; set; } = 50;
    public int TokenBucketSize { get; set; } = 100;
    public int ConcurrentRequestLimit { get; set; } = 50;
}
