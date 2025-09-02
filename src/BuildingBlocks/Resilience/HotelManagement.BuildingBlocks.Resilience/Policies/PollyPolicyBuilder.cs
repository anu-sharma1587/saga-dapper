using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace HotelManagement.BuildingBlocks.Resilience.Policies;

public static class PollyPolicyBuilder
{
    public static IHttpClientBuilder AddResiliencePolicies(
        this IHttpClientBuilder builder,
        string clientName,
        int retryCount = 3,
        int timeoutSeconds = 30,
        int circuitBreakerFailureThreshold = 5,
        int circuitBreakerDurationSeconds = 30)
    {
        return builder
            .AddRetryPolicy(retryCount)
            .AddCircuitBreakerPolicy(circuitBreakerFailureThreshold, circuitBreakerDurationSeconds)
            .AddTimeoutPolicy(timeoutSeconds)
            .AddBulkheadPolicy();
    }

    private static IHttpClientBuilder AddRetryPolicy(
        this IHttpClientBuilder builder,
        int retryCount)
    {
        return builder.AddPolicyHandler(HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                retryCount,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + // Exponential backoff
                               TimeSpan.FromMilliseconds(Random.Shared.Next(0, 1000)) // Jitter
            ));
    }

    private static IHttpClientBuilder AddCircuitBreakerPolicy(
        this IHttpClientBuilder builder,
        int failureThreshold,
        int durationSeconds)
    {
        return builder.AddPolicyHandler(HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: failureThreshold,
                durationOfBreak: TimeSpan.FromSeconds(durationSeconds)
            ));
    }

    private static IHttpClientBuilder AddTimeoutPolicy(
        this IHttpClientBuilder builder,
        int seconds)
    {
        return builder.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(
            seconds,
            TimeoutStrategy.Optimistic
        ));
    }

    private static IHttpClientBuilder AddBulkheadPolicy(
        this IHttpClientBuilder builder,
        int maxParallelization = 100,
        int maxQueuingActions = 25)
    {
        return builder.AddPolicyHandler(Policy.BulkheadAsync<HttpResponseMessage>(
            maxParallelization,
            maxQueuingActions
        ));
    }
}
