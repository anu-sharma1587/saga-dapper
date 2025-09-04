using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace HotelManagement.Services.Availability.Configuration;

public static class ResiliencePolicies
{
    public static IServiceCollection AddResiliencePolicies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ResilienceOptions>()
            .Bind(configuration.GetSection("Resilience"))
            .ValidateDataAnnotations();

        // services.AddHttpClient("default")
        //     .AddPolicyHandler(CreateDefaultPolicy(new ResilienceOptions()));

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> CreateDefaultPolicy(ResilienceOptions options)
    {
        return Policy.WrapAsync(
            CreateRetryPolicy(options),
            CreateCircuitBreakerPolicy(options),
            CreateTimeoutPolicy(options));
    }

    private static IAsyncPolicy<HttpResponseMessage> CreateRetryPolicy(ResilienceOptions options)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutRejectedException>()
            .WaitAndRetryAsync(
                options.MaxRetries,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    // context.GetLogger()?.LogWarning(
                    //     exception,
                    //     "Retry {RetryCount} of {MaxRetries} after {TimeSpan:g}",
                    //     retryCount,
                    //     options.MaxRetries,
                    //     timeSpan);
                });
    }

    private static IAsyncPolicy<HttpResponseMessage> CreateCircuitBreakerPolicy(ResilienceOptions options)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: options.HandledEventsAllowedBeforeBreaking,
                durationOfBreak: TimeSpan.FromSeconds(options.DurationOfBreakInSeconds),
                onBreak: (exception, duration) =>
                {
                    // Log.Warning(
                    //     exception,
                    //     "Circuit breaker opened for {DurationOfBreak:g}",
                    //     duration);
                },
                onReset: () =>
                {
                    // Log.Information("Circuit breaker reset");
                },
                onHalfOpen: () =>
                {
                    // Log.Information("Circuit breaker half-opened");
                });
    }

    private static IAsyncPolicy<HttpResponseMessage> CreateTimeoutPolicy(ResilienceOptions options)
    {
        return Policy.TimeoutAsync<HttpResponseMessage>(
            TimeSpan.FromSeconds(options.TimeoutInSeconds),
            TimeoutStrategy.Optimistic,
            onTimeoutAsync: (context, timeSpan, task) =>
            {
                // context.GetLogger()?.LogWarning(
                //     "Request timed out after {Timeout:g}",
                //     timeSpan);
                return Task.CompletedTask;
            });
    }
}
