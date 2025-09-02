using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace HotelManagement.Services.ApiGateway.RateLimiting;

public static class RateLimitingExtensions
{
    public static IServiceCollection AddCustomRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetSection("RateLimiting").Get<RateLimitingOptions>() ?? new RateLimitingOptions();

        services.AddRateLimiter(rateLimiter =>
        {
            // Global rate limiting policy
            rateLimiter.AddFixedWindowLimiter("GlobalLimiter", opt =>
            {
                opt.PermitLimit = options.GlobalRequestsPerSecond;
                opt.Window = TimeSpan.FromSeconds(1);
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = options.TokenBucketSize;
            });

            // Authentication endpoints policy
            rateLimiter.AddTokenBucketLimiter("AuthLimiter", opt =>
            {
                opt.TokenLimit = options.AuthRequestsPerMinute;
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = options.TokenBucketSize;
                opt.ReplenishmentPeriod = TimeSpan.FromMinutes(1);
                opt.TokensPerPeriod = options.AuthRequestsPerMinute;
                opt.AutoReplenishment = true;
            });

            // Search endpoints policy
            rateLimiter.AddSlidingWindowLimiter("SearchLimiter", opt =>
            {
                opt.PermitLimit = options.SearchRequestsPerMinute;
                opt.Window = TimeSpan.FromMinutes(1);
                opt.SegmentsPerWindow = 6; // 10-second segments
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = options.TokenBucketSize;
            });

            // Booking endpoints policy
            rateLimiter.AddConcurrencyLimiter("BookingLimiter", opt =>
            {
                opt.PermitLimit = options.ConcurrentRequestLimit;
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = options.TokenBucketSize;
            });

            // User endpoints policy
            rateLimiter.AddTokenBucketLimiter("UserLimiter", opt =>
            {
                opt.TokenLimit = options.UserRequestsPerMinute;
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = options.TokenBucketSize;
                opt.ReplenishmentPeriod = TimeSpan.FromMinutes(1);
                opt.TokensPerPeriod = options.UserRequestsPerMinute;
                opt.AutoReplenishment = true;
            });

            // Configure the policy resolution
            rateLimiter.OnRejected = async (context, token) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter =
                        ((int)retryAfter.TotalSeconds).ToString();
                }

                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    error = "Too many requests. Please try again later.",
                    retryAfter = retryAfter?.TotalSeconds ?? 60
                }, token);
            };
        });

        return services;
    }

    public static IEndpointConventionBuilder RequireGlobalRateLimit(this IEndpointConventionBuilder builder)
    {
        return builder.RequireRateLimiting("GlobalLimiter");
    }

    public static IEndpointConventionBuilder RequireAuthRateLimit(this IEndpointConventionBuilder builder)
    {
        return builder.RequireRateLimiting("AuthLimiter");
    }

    public static IEndpointConventionBuilder RequireSearchRateLimit(this IEndpointConventionBuilder builder)
    {
        return builder.RequireRateLimiting("SearchLimiter");
    }

    public static IEndpointConventionBuilder RequireBookingRateLimit(this IEndpointConventionBuilder builder)
    {
        return builder.RequireRateLimiting("BookingLimiter");
    }

    public static IEndpointConventionBuilder RequireUserRateLimit(this IEndpointConventionBuilder builder)
    {
        return builder.RequireRateLimiting("UserLimiter");
    }
}
