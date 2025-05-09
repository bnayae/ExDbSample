namespace AwsProvider;

internal static partial class Log
{
    [LoggerMessage(LogLevel.Critical, "Error processing {message}")]
    public static partial void ErrorProcessingMessage(this ILogger logger, string message, Exception? exception);
}
