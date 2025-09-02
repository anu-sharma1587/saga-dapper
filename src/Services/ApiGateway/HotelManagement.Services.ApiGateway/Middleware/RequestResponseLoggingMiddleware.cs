using System.Diagnostics;
using System.Text;
using Microsoft.IO;

namespace HotelManagement.Services.ApiGateway.Middleware;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
    private readonly RecyclableMemoryStreamManager _streamManager;

    public RequestResponseLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
        _streamManager = new RecyclableMemoryStreamManager();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var activityId = Activity.Current?.Id ?? context.TraceIdentifier;

        // Log the request
        await LogRequest(context, activityId);

        // Capture the response
        var originalBodyStream = context.Response.Body;
        await using var responseStream = _streamManager.GetStream();
        context.Response.Body = responseStream;

        try
        {
            await _next(context);
            await LogResponse(context, activityId, responseStream);
        }
        finally
        {
            responseStream.Position = 0;
            await responseStream.CopyToAsync(originalBodyStream);
            context.Response.Body = originalBodyStream;
        }
    }

    private async Task LogRequest(HttpContext context, string activityId)
    {
        context.Request.EnableBuffering();

        var requestBody = string.Empty;
        if (context.Request.Body.CanRead)
        {
            requestBody = await ReadStreamAsync(context.Request.Body);
            context.Request.Body.Position = 0;
        }

        var requestInfo = new
        {
            ActivityId = activityId,
            Timestamp = DateTime.UtcNow,
            IP = context.Connection.RemoteIpAddress?.ToString(),
            Protocol = context.Request.Protocol,
            Method = context.Request.Method,
            Scheme = context.Request.Scheme,
            Host = context.Request.Host.ToString(),
            Path = context.Request.Path,
            QueryString = context.Request.QueryString.ToString(),
            Headers = context.Request.Headers
                .Where(h => !h.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
                .ToDictionary(h => h.Key, h => h.Value.ToString()),
            Body = requestBody.Length > 4000 ? requestBody[..4000] + "..." : requestBody
        };

        _logger.LogInformation("HTTP Request Information: {@RequestInfo}", requestInfo);
    }

    private async Task LogResponse(HttpContext context, string activityId, Stream responseStream)
    {
        responseStream.Position = 0;
        var responseBody = await ReadStreamAsync(responseStream);

        var responseInfo = new
        {
            ActivityId = activityId,
            Timestamp = DateTime.UtcNow,
            StatusCode = context.Response.StatusCode,
            Headers = context.Response.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
            Body = responseBody.Length > 4000 ? responseBody[..4000] + "..." : responseBody
        };

        _logger.LogInformation("HTTP Response Information: {@ResponseInfo}", responseInfo);
    }

    private static async Task<string> ReadStreamAsync(Stream stream)
    {
        const int readChunkSize = 4096;
        using var textWriter = new StringWriter();
        using var reader = new StreamReader(stream, Encoding.UTF8, true, readChunkSize, true);
        
        var text = await reader.ReadToEndAsync();
        return text;
    }
}

public static class RequestResponseLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestResponseLoggingMiddleware>();
    }
}
