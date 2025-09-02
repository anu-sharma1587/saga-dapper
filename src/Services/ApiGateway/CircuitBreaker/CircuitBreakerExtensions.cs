using Polly;
using Polly.CircuitBreaker;
using Yarp.ReverseProxy.Transforms;

namespace HotelManagement.Services.ApiGateway.CircuitBreaker;

public class CircuitBreakerOptions
{
    public int FailureThreshold { get; set; } = 5;
    public int SamplingDurationSeconds { get; set; } = 30;
    public int MinimumThroughput { get; set; } = 3;
    public int DurationOfBreakSeconds { get; set; } = 15;
    public int RetryCount { get; set; } = 3;
    public int RetryDelayMilliseconds { get; set; } = 200;
}

public static class CircuitBreakerExtensions
{
    private static readonly Dictionary<string, ICircuitBreakerPolicy<HttpResponseMessage>> Circuits = new();

    public static IServiceCollection AddCircuitBreaker(this IServiceCollection services, IConfiguration configuration)
    {
        var options = configuration.GetSection("CircuitBreaker").Get<CircuitBreakerOptions>() ?? new CircuitBreakerOptions();

        services.AddHttpClient("CircuitBreakerClient")
            .AddTransientHttpErrorPolicy(builder => builder
                .WaitAndRetryAsync(
                    options.RetryCount,
                    retryAttempt => TimeSpan.FromMilliseconds(options.RetryDelayMilliseconds * Math.Pow(2, retryAttempt - 1)),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        var logger = services.BuildServiceProvider().GetService<ILogger<CircuitBreakerExtensions>>();
                        logger?.LogWarning(exception, "Retry {RetryCount} after {TimeSpan}ms", retryCount, timeSpan.TotalMilliseconds);
                    }))
            .AddTransientHttpErrorPolicy(builder => builder
                .AdvancedCircuitBreakerAsync(
                    failureThreshold: options.FailureThreshold,
                    samplingDuration: TimeSpan.FromSeconds(options.SamplingDurationSeconds),
                    minimumThroughput: options.MinimumThroughput,
                    durationOfBreak: TimeSpan.FromSeconds(options.DurationOfBreakSeconds),
                    onBreak: (exception, duration) =>
                    {
                        var logger = services.BuildServiceProvider().GetService<ILogger<CircuitBreakerExtensions>>();
                        logger?.LogError(exception, "Circuit breaker opened for {Duration}s", duration.TotalSeconds);
                    },
                    onReset: () =>
                    {
                        var logger = services.BuildServiceProvider().GetService<ILogger<CircuitBreakerExtensions>>();
                        logger?.LogInformation("Circuit breaker reset");
                    },
                    onHalfOpen: () =>
                    {
                        var logger = services.BuildServiceProvider().GetService<ILogger<CircuitBreakerExtensions>>();
                        logger?.LogInformation("Circuit breaker half-open");
                    }));

        return services;
    }

    public static IApplicationBuilder UseCircuitBreaker(this IApplicationBuilder app)
    {
        var logger = app.ApplicationServices.GetService<ILogger<CircuitBreakerExtensions>>();

        // Add circuit breaker transform to YARP
        var transform = new CircuitBreakerTransform(logger);
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapReverseProxy(proxyPipeline =>
            {
                proxyPipeline.Use((context, next) =>
                {
                    var circuitKey = context.Request.Path.Value?.Split('/')[2] ?? "default";
                    
                    if (!Circuits.ContainsKey(circuitKey))
                    {
                        var options = context.RequestServices
                            .GetService<IConfiguration>()
                            ?.GetSection("CircuitBreaker")
                            .Get<CircuitBreakerOptions>() ?? new CircuitBreakerOptions();

                        Circuits[circuitKey] = Policy<HttpResponseMessage>
                            .Handle<HttpRequestException>()
                            .Or<TimeoutException>()
                            .AdvancedCircuitBreakerAsync(
                                failureThreshold: options.FailureThreshold,
                                samplingDuration: TimeSpan.FromSeconds(options.SamplingDurationSeconds),
                                minimumThroughput: options.MinimumThroughput,
                                durationOfBreak: TimeSpan.FromSeconds(options.DurationOfBreakSeconds));
                    }

                    var circuit = Circuits[circuitKey];
                    
                    if (circuit.CircuitState == CircuitState.Open)
                    {
                        context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                        return Task.CompletedTask;
                    }

                    return next(context);
                });
            });
        });

        return app;
    }
}

public class CircuitBreakerTransform : RequestTransform
{
    private readonly ILogger? _logger;

    public CircuitBreakerTransform(ILogger? logger)
    {
        _logger = logger;
    }

    public override async ValueTask ApplyAsync(RequestTransformContext context)
    {
        if (context.HttpContext.Response.StatusCode >= 500)
        {
            _logger?.LogWarning("Service returned error {StatusCode}", context.HttpContext.Response.StatusCode);
            throw new HttpRequestException($"Service returned {context.HttpContext.Response.StatusCode}");
        }

        await ValueTask.CompletedTask;
    }
}
