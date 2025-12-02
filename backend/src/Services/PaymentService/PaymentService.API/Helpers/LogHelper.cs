using Microsoft.Extensions.Logging;

namespace PaymentService.API.Helpers;

public static class LogHelper
{
    public static void LogStartup(ILogger logger, string serviceName, int port)
    {
        logger.LogInformation("═══════════════════════════════════════════════════════════");
        logger.LogInformation("🚀 {ServiceName} STARTING...", serviceName);
        logger.LogInformation("📍 Port: {Port}", port);
        logger.LogInformation("🕐 Time: {Time}", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC"));
        logger.LogInformation("═══════════════════════════════════════════════════════════");
    }

    public static void LogReady(ILogger logger, string serviceName)
    {
        logger.LogInformation("═══════════════════════════════════════════════════════════");
        logger.LogInformation("✅ {ServiceName} IS READY!", serviceName);
        logger.LogInformation("💳 Payment processing: Active");
        logger.LogInformation("🐰 RabbitMQ consumer: Listening");
        logger.LogInformation("📊 Swagger UI: /swagger");
        logger.LogInformation("═══════════════════════════════════════════════════════════");
    }

    public static void LogShutdown(ILogger logger, string serviceName)
    {
        logger.LogInformation("═══════════════════════════════════════════════════════════");
        logger.LogInformation("🛑 {ServiceName} SHUTTING DOWN...", serviceName);
        logger.LogInformation("🕐 Time: {Time}", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC"));
        logger.LogInformation("═══════════════════════════════════════════════════════════");
    }
}
