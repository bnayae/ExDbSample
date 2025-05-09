using Amazon.SQS;
using Amazon.SQS.Model;
using AwsProvider;
using Microsoft.Extensions.Hosting;
#pragma warning disable S101 // Types should be named in PascalCase


namespace Microsoft.Extensions;


/// <summary>
/// Base class for SQS processors hosting.
/// </summary>
/// <typeparam name="T"></typeparam>
internal abstract class SQSProcessorBase<T> : BackgroundService
{
    private readonly ILogger _logger;
    private readonly Func<T, CancellationToken, Task> _messageHandler;
    private readonly string _queueName;

    protected SQSProcessorBase(ILogger logger,
                        Func<T, CancellationToken, Task> messageHandler,
                        string queueName)
    {
        _logger = logger;
        _messageHandler = messageHandler;
        _queueName = queueName;
    }

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
                    var bodyJson = msg.Body;
                    T message = JsonSerializer.Deserialize<T>(bodyJson) ?? throw new JsonException("Fail to deserialize message");
                    await _messageHandler(message, stoppingToken);
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