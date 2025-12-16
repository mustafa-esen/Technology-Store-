using Serilog;

namespace BasketService.API.Helpers;

public static class LogHelper
{
    private const string Rocket = "🚀";
    private const string CheckEmoji = "✅";
    private const string ErrorEmoji = "❌";
    private const string WarningEmoji = "⚠️";
    private const string InfoEmoji = "ℹ️";
    private const string BasketEmoji = "🛒";
    private const string ApiEmoji = "🌐";
    private const string TimerEmoji = "⏱️";
    private const string ConfigEmoji = "⚙️";
    private const string ShutdownEmoji = "🛑";
    private const string PackageEmoji = "📦";
    private const string SuccessEmoji = "🎉";
    private const string ProcessEmoji = "⚡";
    private const string RedisEmoji = "💎";

    public static void LogStartup(string serviceName, string version, string environment)
    {
        var banner = $@"
╔══════════════════════════════════════════════════════════════╗
║  {Rocket}  {serviceName.PadRight(50)}  {Rocket}  ║
║  Version: {version.PadRight(47)}    ║
║  Environment: {environment.PadRight(43)}    ║
║  Starting Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}                           ║
╚══════════════════════════════════════════════════════════════╝";

        Log.Information(banner);
        Log.Information("{Emoji} Application is starting up...", Rocket);
    }

    public static void LogConfiguration(string message)
    {
        Log.Information("{Emoji} Configuration: {Message}", ConfigEmoji, message);
    }

    public static void LogRedis(string message)
    {
        Log.Information("{Emoji} Redis: {Message}", RedisEmoji, message);
    }

    public static void LogApi(string message)
    {
        Log.Information("{Emoji} API: {Message}", ApiEmoji, message);
    }

    public static void LogSuccess(string message)
    {
        Log.Information("{Emoji} Success: {Message}", CheckEmoji, message);
    }

    public static void LogProcess(string message)
    {
        Log.Information("{Emoji} Process: {Message}", ProcessEmoji, message);
    }

    public static void LogPackage(string packageName, string status)
    {
        Log.Information("{Emoji} Package: {PackageName} - {Status}", PackageEmoji, packageName, status);
    }

    public static void LogTimer(string operationName, long elapsedMs)
    {
        Log.Information("{Emoji} {OperationName}: {ElapsedMs}ms", TimerEmoji, operationName, elapsedMs);
    }

    public static void LogShutdown()
    {
        var shutdownBanner = $@"
╔══════════════════════════════════════════════════════════════╗
║  {ShutdownEmoji}  Application is shutting down gracefully...          {ShutdownEmoji}  ║
║  Shutdown Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}                          ║
║  Thank you for using Basket Service! {SuccessEmoji}                    ║
╚══════════════════════════════════════════════════════════════╝";

        Log.Information(shutdownBanner);
        Log.Information("{Emoji} Application has stopped", ShutdownEmoji);
    }

    public static void LogError(Exception ex, string message)
    {
        Log.Error(ex, "{Emoji} Error: {Message}", ErrorEmoji, message);
    }

    public static void LogWarning(string message)
    {
        Log.Warning("{Emoji} Warning: {Message}", WarningEmoji, message);
    }

    public static void LogInfo(string message)
    {
        Log.Information("{Emoji} Info: {Message}", InfoEmoji, message);
    }
}
