using Amazon.SQS;
using Amazon.SQS.Model;
using AwsProvider;
using EvDb.Adapters.Store.Internals;
using EvDb.Core;
using EvDb.Core.Adapters;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
#pragma warning disable S101 // Types should be named in PascalCase
#pragma warning disable CA1303 // Do not pass literals as localized parameters


namespace Microsoft.Extensions;


/// <summary>
/// Base class for SQS processors hosting.
/// </summary>
/// <typeparam name="T"></typeparam>
internal abstract class SQSProcessorBase<T> : BackgroundService
{
    private readonly ILogger _logger;
    private readonly Func<T, CancellationToken, Task> _messageHandler;
    private readonly Func<IEvDbMessageMeta, bool> _filter;
    private readonly string _queueName;

    /// <summary>
    /// Base class for SQS processors hosting.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="messageHandler"></param>
    /// <param name="filter">Filter function to determine if the message should be processed.</param>
    /// <param name="queueName"></param>
    protected SQSProcessorBase(ILogger logger,
                        Func<T, CancellationToken, Task> messageHandler,
                        Func<IEvDbMessageMeta, bool> filter,
                        string queueName)
    {
        _logger = logger;
        _messageHandler = messageHandler;
        _filter = filter;
        _queueName = queueName;
    }

    /// <summary>
    /// Executes the SQS processor.
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    /// <exception cref="JsonException"></exception>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using AmazonSQSClient sqsClient = AWSProviderFactory.CreateSQSClient();

        string queueUrl = await sqsClient.GetOrCreateQueueUrlAsync(_queueName);

        while (!stoppingToken.IsCancellationRequested)
        {
            var received = await sqsClient.ReceiveMessageAsync(new ReceiveMessageRequest
            {
                QueueUrl = queueUrl,
                MaxNumberOfMessages = 1,
                WaitTimeSeconds = 5
            }, stoppingToken);

            foreach (var msg in received.Messages ?? [])
            {
                try
                {
                    string bodyJson = msg.Body;
                    var parsed = JsonDocument.Parse(bodyJson);
                    var rawMessage = parsed.RootElement.GetProperty("Message").ToString();
                    var doc = BsonDocument.Parse(rawMessage);

                    EvDbMessageRecord message = doc.ToMessageRecord();
                    IEvDbMessageMeta meta = message.GetMetadata();

                    // TODO: Filter Metrics (include vs. exclude)
                    if (_filter.Invoke(meta))
                    {
                        T body = JsonSerializer.Deserialize<T>(message.Payload) ?? throw new JsonException($"failed to serialize `{typeof(T).Name}`");
                        _logger.ProcessingOutboxMessage(meta.StreamCursor.ToString(), meta.EventType, meta.MessageType);

                        await _messageHandler(body, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.ErrorProcessingMessage(msg.Body, ex);
                    throw;
                }

                await sqsClient.DeleteMessageAsync(queueUrl, msg.ReceiptHandle, stoppingToken);
            }
        }
    }
}
