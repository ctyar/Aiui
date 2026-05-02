namespace Web;

internal static partial class LoggerExtensions
{
    [LoggerMessage(LogLevel.Warning, "Request logging failed")]
    public static partial void RequestFailed(this ILogger logger, Exception ex);
}
