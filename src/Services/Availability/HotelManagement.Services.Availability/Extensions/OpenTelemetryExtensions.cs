using System.Diagnostics;
using HotelManagement.Services.Availability.Monitoring;
using OpenTelemetry.Trace;

namespace HotelManagement.Services.Availability.Extensions;

public static class OpenTelemetryExtensions
{
    public static IServiceCollection AddOpenTelemetry(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics
                    .AddMeter("HotelManagement.Availability")
                    .AddRuntimeMetrics()
                    .AddProcessMetrics()
                    .AddHttpClientMetrics();

                // Configure exporters based on configuration
                var prometheusEndpoint = configuration["OpenTelemetry:Prometheus:Endpoint"];
                if (!string.IsNullOrEmpty(prometheusEndpoint))
                {
                    metrics.AddPrometheusExporter(options =>
                    {
                        options.ScrapeEndpointPath = prometheusEndpoint;
                    });
                }
            })
            .WithTracing(tracing =>
            {
                tracing
                    .AddSource("HotelManagement.Availability")
                    .SetResourceBuilder(OpenTelemetry.Resources.ResourceBuilder.CreateDefault()
                        .AddService("AvailabilityService"))
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddRedisInstrumentation();

                // Configure exporters based on configuration
                var otlpEndpoint = configuration["OpenTelemetry:OTLP:Endpoint"];
                if (!string.IsNullOrEmpty(otlpEndpoint))
                {
                    tracing.AddOtlpExporter(options => options.Endpoint = new Uri(otlpEndpoint));
                }

                var jaegerEndpoint = configuration["OpenTelemetry:Jaeger:Endpoint"];
                if (!string.IsNullOrEmpty(jaegerEndpoint))
                {
                    tracing.AddJaegerExporter(options => options.AgentHost = jaegerEndpoint);
                }
            });

        services.AddSingleton<AvailabilityMetrics>();
        
        return services;
    }
}
