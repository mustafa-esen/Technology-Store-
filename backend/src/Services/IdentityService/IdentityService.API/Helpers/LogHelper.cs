using Serilog;

namespace IdentityService.API.Helpers;

public static class LogHelper
{
    private const string Rocket = "🚀";
    private const string CheckEmoji = "✅";
    private const string ErrorEmoji = "❌";
    private const string WarningEmoji = "⚠️";
    private const string InfoEmoji = "ℹ️";
    private const string DatabaseEmoji = "💾";
    private const string ApiEmoji = "🌐";
    private const string TimerEmoji = "⏱️";
    private const string ConfigEmoji = "⚙️";
    private const string ShutdownEmoji = "🛑";
    private const string PackageEmoji = "📦";
    private const string UserEmoji = "👤";
    private const string SuccessEmoji = "🎉";
    private const string ProcessEmoji = "⚡";
    private const string SecurityEmoji = "🔒";
    private const string TokenEmoji = "🎫";
    private const string AuthEmoji = "🔐";

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

    public static void LogDatabase(string message)
    {
        Log.Information("{Emoji} Database: {Message}", DatabaseEmoji, message);
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

    public static void LogRequest(string method, string path, string username = "Anonymous")
    {
        Log.Information("{Emoji} Request: {Method} {Path} by {User}", UserEmoji, method, path, username);
    }

    public static void LogRequestCompleted(string method, string path, int statusCode, long elapsedMs)
    {
        var emoji = statusCode < 400 ? SuccessEmoji : ErrorEmoji;
        Log.Information("{Emoji} Request Completed: {Method} {Path} - {StatusCode} in {ElapsedMs}ms",
            emoji, method, path, statusCode, elapsedMs);
    }

    public static void LogAuthentication(string action, string email, bool success)
    {
        var emoji = success ? AuthEmoji : ErrorEmoji;
        Log.Information("{Emoji} Authentication: {Action} for {Email} - {Status}",
            emoji, action, email, success ? "Success" : "Failed");
    }

    public static void LogTokenGeneration(string email)
    {
        Log.Information("{Emoji} Token generated for {Email}", TokenEmoji, email);
    }

    public static void LogTokenRefresh(string email)
    {
        Log.Information("{Emoji} Token refreshed for {Email}", TokenEmoji, email);
    }

    public static void LogSecurity(string message)
    {
        Log.Information("{Emoji} Security: {Message}", SecurityEmoji, message);
    }

    public static void LogShutdown()
    {
        var shutdownBanner = $@"
╔══════════════════════════════════════════════════════════════╗
║  {ShutdownEmoji}  Application is shutting down gracefully...          {ShutdownEmoji}  ║
║  Shutdown Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}                          ║
║  Thank you for using Identity Service! {SuccessEmoji}                   ║
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

    public static void LogTimer(string operation, long milliseconds)
    {
        Log.Information("{Emoji} Timer: {Operation} completed in {Milliseconds}ms", TimerEmoji, operation, milliseconds);
    }
}
