namespace AwsProvider;

internal static partial class Log
{
    [LoggerMessage(LogLevel.Critical, "Error processing {message}")]
    public static partial void ErrorProcessingMessage(this ILogger logger, string message, Exception? exception);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Processing Outbox message: \n          {StreamCursor}\n          Event Type: {EventType}\n          Message Type: {MessageType}")]
    public static partial void ProcessingOutboxMessage(this ILogger logger, string streamCursor, string eventType, string messageType);
}
