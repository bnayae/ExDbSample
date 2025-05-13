using Amazon.SimpleNotificationService.Model;
using Amazon.SQS.Model;
using EvDb.Adapters.Store.Internals;
using EvDb.Core;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry;
#pragma warning disable CA1848 // Use the LoggerMessage delegates
#pragma warning disable CA2254 // Template should be a static expression
#pragma warning disable S2629 // Logging templates should be constant

namespace Microsoft.Extensions;

internal static class Extensions
{
    // private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;

    private const string MONGODB_ENDPOINT = "mongodb://localhost:27017";

    #region StartListenToChangeStreamAsync

    public static async Task StartListenToChangeStreamAsync(this IMongoCollection<BsonDocument> collection,
                                                            Func<string, Task> onChange,
                                                            CancellationToken cancellationToken)
    {
        using var changeStream = await collection.WatchAsync(cancellationToken: cancellationToken);

        while (!cancellationToken.IsCancellationRequested && await changeStream.MoveNextAsync(cancellationToken))
        {
            foreach (var change in changeStream.Current)
            {
                var message = change.FullDocument.ToJson() ?? "MISSING";
                await onChange(message);
            }
        }
    }

    #endregion //  StartListenToChangeStreamAsync

    #region OuboxTo

    /// <summary>
    /// Listens to the outbox and sends messages to SQS.
    /// </summary>
    /// <param name="setting"></param>
    /// <param name="logger"></param>
    /// <param name="visibilityTimeout">Visibility timeout for the SQS message.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task ListenToOutbox(this StreamSinkSetting setting,
                                            ILogger logger,
                                            TimeSpan visibilityTimeout,
                                            CancellationToken cancellationToken)
    {
        await setting.ListenToOutbox(logger, _ => true, visibilityTimeout, cancellationToken);
    }

    /// <summary>
    /// Listens to the outbox and sends messages to SQS based on a filter.
    /// </summary>
    /// <param name="setting"></param>
    /// <param name="logger"></param>
    /// <param name="filter">Filter function to determine if the message should be processed.</param>
    /// <param name="visibilityTimeout">Visibility timeout for the SQS message.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task ListenToOutbox(this StreamSinkSetting setting,
                                            ILogger logger,
                                            Func<IEvDbMessageMeta, bool> filter,
                                            TimeSpan visibilityTimeout,
                                            CancellationToken cancellationToken)
    {
        (string dbName, string collectionName, string streamName, string queueName) = setting;

        using var snsClient = AWSProviderFactory.CreateSNSClient();
        using var sqsClient = AWSProviderFactory.CreateSQSClient();

        // Create SNS topic if it doesn't exist
        string topicArn = await snsClient.GetOrCreateStreamAsync(streamName);


        // Create SQS queue if it doesn't exist
        string queueUrl = await sqsClient.GetOrCreateQueueUrlAsync(queueName, visibilityTimeout);
        string queueArn = await sqsClient.GetQueueARNAsync(queueUrl);

        // Allow SNS to send to SQS (Policy)
        await sqsClient.SetSNSToSQSPolicyAsync(topicArn, queueUrl, queueArn);
        // Subscribe SQS to SNS topic
        await snsClient.AllowSNSToSendToSQSAsync(topicArn, queueArn);

        var mongoClientSettings = MongoClientSettings.FromConnectionString(MONGODB_ENDPOINT);
        mongoClientSettings.ClusterConfigurator = cb =>
        {
            cb.Subscribe(new DiagnosticsActivityEventSubscriber());
        };
        using var mongoClient = new MongoClient(mongoClientSettings);
        var database = mongoClient.GetDatabase(dbName);
        IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(collectionName);

        logger.LogDebug($"""
            Attaching 
                  to:
                      DB:{dbName}  
                      Collection:{collectionName} 
                  Into:
                      SQS:{queueName} via 
                      SNS:{streamName}
            """);


        await collection.StartListenToChangeStreamAsync(async change =>
        {
            IEvDbMessageMeta meta = ToMetadata(change);
            if (filter != null && !filter(meta))
            {
                return;
            }

            logger.LogDebug($"""
                Sending message 
                      to:
                          DB:{dbName}  
                          Collection:{collectionName} 
                      Into:
                          SQS:{queueName} via 
                          SNS:{streamName}
                """);

            //var parentContext = new ActivityContext(
            //        ActivityTraceId.CreateFromString(meta.TraceId),
            //        ActivitySpanId.CreateFromString(meta.SpanId),
            //        ActivityTraceFlags.Recorded
            //    );

            //// Create a propagation context
            //var propagationContext = new PropagationContext(
            //    parentContext,
            //    Baggage.Current
            //);

            var request = new PublishRequest
            {
                TopicArn = topicArn,
                Message = change
            };

            await snsClient.PublishAsync(request);
        }, cancellationToken);

    }

    /// <summary>
    /// Listens to the outbox and sends messages to SQS.
    /// </summary>
    /// <param name="setting"></param>
    /// <param name="logger"></param>
    /// <param name="visibilityTimeout">Visibility timeout for the SQS message.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task ListenToOutbox(this QueueSinkSetting setting, 
                                            ILogger logger,
                                            TimeSpan visibilityTimeout,
                                            CancellationToken cancellationToken)
    {
        await setting.ListenToOutbox(logger, _ => true, visibilityTimeout, cancellationToken);
    }

    /// <summary>
    /// Listens to the outbox and sends messages to SQS based on a filter.
    /// </summary>
    /// <param name="setting"></param>
    /// <param name="logger"></param>
    /// <param name="filter">Filter function to determine if the message should be processed.</param>
    /// <param name="visibilityTimeout">Visibility timeout for the SQS message.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task ListenToOutbox(this QueueSinkSetting setting,
                                            ILogger logger,
                                            Func<IEvDbMessageMeta, bool> filter,
                                            TimeSpan visibilityTimeout,
                                            CancellationToken cancellationToken)
    {

        (string dbName, string collectionName, string queueName) = setting;

        using var sqsClient = AWSProviderFactory.CreateSQSClient();

        // Create SQS queue if it doesn't exist
        string queueUrl = await sqsClient.GetOrCreateQueueUrlAsync(queueName, visibilityTimeout);


        var mongoClientSettings = MongoClientSettings.FromConnectionString(MONGODB_ENDPOINT);
        mongoClientSettings.ClusterConfigurator = cb =>
        {
            cb.Subscribe(new DiagnosticsActivityEventSubscriber());
        };
        using var mongoClient = new MongoClient(mongoClientSettings);

        var database = mongoClient.GetDatabase(dbName);
        IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(collectionName);

        Console.ForegroundColor = ConsoleColor.Cyan;
        logger.LogDebug($"Attaching DB:{dbName} on Collection:{collectionName} to SQS:{queueName}");
        Console.ResetColor();
        await collection.StartListenToChangeStreamAsync(async change =>
        {
            IEvDbMessageMeta meta = ToMetadata(change);
            if (filter != null && !filter(meta))
            {
                return;
            }

            logger.LogDebug($"""
                Sending message 
                      to:
                          DB:{dbName}  
                          Collection:{collectionName} 
                      Into:
                          SQS:{queueName} 
                """);
            var request = new SendMessageRequest
            {
                QueueUrl = queueUrl,
                MessageBody = change
            };
            await sqsClient.SendMessageAsync(request);
        }, cancellationToken);

    }

    #endregion //  OuboxTo

    #region ToMetadata

    /// <summary>
    /// Checks the filter for the message.
    /// </summary>
    /// <param name="change"></param>
    /// <returns></returns>
    private static IEvDbMessageMeta ToMetadata(string change)
    {
        var doc = BsonDocument.Parse(change);

        IEvDbMessageMeta meta = doc.ToMessageMeta();
        return meta;
    }

    #endregion //  ToMetadata
}
