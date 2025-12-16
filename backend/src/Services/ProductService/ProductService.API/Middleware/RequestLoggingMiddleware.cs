using ProductService.API.Helpers;
using System.Diagnostics;

namespace ProductService.API.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var method = context.Request.Method;
        var path = context.Request.Path;

        try
        {
            LogHelper.LogRequest(method, path);

            await _next(context);

            stopwatch.Stop();
            LogHelper.LogRequestCompleted(method, path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            LogHelper.LogError(ex, $"Request failed: {method} {path}");
            throw;
        }
    }
}
