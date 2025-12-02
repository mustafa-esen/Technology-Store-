using System.Diagnostics;

namespace PaymentService.API.Middleware;

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
        var requestPath = context.Request.Path;
        var requestMethod = context.Request.Method;

        try
        {
            _logger.LogInformation("➡️ {Method} {Path} started", requestMethod, requestPath);

            await _next(context);

            stopwatch.Stop();
            var statusCode = context.Response.StatusCode;

            if (statusCode >= 200 && statusCode < 300)
            {
                _logger.LogInformation("✅ {Method} {Path} completed in {ElapsedMilliseconds}ms with status {StatusCode}",
                    requestMethod, requestPath, stopwatch.ElapsedMilliseconds, statusCode);
            }
            else if (statusCode >= 400 && statusCode < 500)
            {
                _logger.LogWarning("⚠️ {Method} {Path} completed in {ElapsedMilliseconds}ms with status {StatusCode}",
                    requestMethod, requestPath, stopwatch.ElapsedMilliseconds, statusCode);
            }
            else if (statusCode >= 500)
            {
                _logger.LogError("❌ {Method} {Path} failed in {ElapsedMilliseconds}ms with status {StatusCode}",
                    requestMethod, requestPath, stopwatch.ElapsedMilliseconds, statusCode);
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "❌ {Method} {Path} threw exception after {ElapsedMilliseconds}ms",
                requestMethod, requestPath, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
