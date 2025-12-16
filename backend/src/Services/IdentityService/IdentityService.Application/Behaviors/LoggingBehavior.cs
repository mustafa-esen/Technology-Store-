using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace IdentityService.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestType = requestName.Contains("Command") ? "Command" : "Query";
        var emoji = requestName.Contains("Command") ? "📝" : "🔍";

        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation("{Emoji} Handling {RequestType}: {RequestName}", emoji, requestType, requestName);
        _logger.LogDebug("Request Details: {@Request}", request);

        try
        {
            var response = await next();

            stopwatch.Stop();

            if (stopwatch.ElapsedMilliseconds > 3000)
            {
                _logger.LogWarning("⚠️ Slow {RequestType}: {RequestName} took {ElapsedMilliseconds}ms",
                    requestType, requestName, stopwatch.ElapsedMilliseconds);
            }
            else
            {
                _logger.LogInformation("✅ {RequestType} completed: {RequestName} in {ElapsedMilliseconds}ms",
                    requestType, requestName, stopwatch.ElapsedMilliseconds);
            }

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex,
                "❌ Error in {RequestType}: {RequestName} after {ElapsedMilliseconds}ms",
                requestType,
                requestName,
                stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
