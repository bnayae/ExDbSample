using Amazon.SQS;
using Amazon.SQS.Model;
using AwsProvider;
using Core.Abstractions;
using Microsoft.Extensions.Hosting;


namespace Microsoft.Extensions;

internal class SqsProcessor<T> : BackgroundService
{
    private readonly ILogger<SqsProcessor<T>> _logger;
    private readonly ICommandEntry<T> _commandEntry;
    private readonly string _queueName;

    public SqsProcessor(ILogger<SqsProcessor<T>> logger,
                        ICommandEntry<T> commandEntry,
                        string queueName)
    {
        _logger = logger;
        _commandEntry = commandEntry;
        _queueName = queueName;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using AmazonSQSClient sqsClient = AwsProviderFactory.CreateSqsClient();

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
                    await _commandEntry.ProcessAsync(message, stoppingToken);
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