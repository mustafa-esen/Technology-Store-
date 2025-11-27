using FluentValidation;
using System.Net;
using System.Text.Json;

namespace IdentityService.API.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message, errors) = exception switch
        {
            ValidationException validationException => (
                (int)HttpStatusCode.BadRequest,
                "Validation failed",
                validationException.Errors.Select(e => new
                {
                    property = e.PropertyName,
                    message = e.ErrorMessage
                }).ToArray()
            ),
            InvalidOperationException invalidOperationException => (
                (int)HttpStatusCode.BadRequest,
                invalidOperationException.Message,
                Array.Empty<object>()
            ),
            ArgumentException argumentException => (
                (int)HttpStatusCode.BadRequest,
                argumentException.Message,
                Array.Empty<object>()
            ),
            KeyNotFoundException => (
                (int)HttpStatusCode.NotFound,
                "Resource not found",
                Array.Empty<object>()
            ),
            UnauthorizedAccessException unauthorizedException => (
                (int)HttpStatusCode.Unauthorized,
                unauthorizedException.Message,
                Array.Empty<object>()
            ),
            _ => (
                (int)HttpStatusCode.InternalServerError,
                "An internal server error occurred",
                Array.Empty<object>()
            )
        };

        context.Response.StatusCode = statusCode;

        var response = new
        {
            statusCode,
            message,
            errors,
            timestamp = DateTime.UtcNow
        };

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}
