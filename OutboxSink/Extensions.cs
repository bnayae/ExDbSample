using Amazon.Auth.AccessControlPolicy;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Net;
using System.Text.Json;

namespace Microsoft.Extensions;

internal static class Extensions
{
    private readonly static string MONGODB_ENDPOINT = "mongodb://localhost:27017";


    #region OuboxTo

    public static async Task ListenToOubox(this StreamSinkSetting setting ,CancellationToken cancellationToken)
    {
        (string dbName, string collectionName, string streamName, string queueName) = setting;

        using var snsClient = AwsProviderFactory.CreateSnsClient();
        using var sqsClient = AwsProviderFactory.CreateSqsClient();

        // Create SNS topic if it doesn't exist
        string topicArn = await snsClient.GetOrCreateTopicAsync(streamName);


        // Create SQS queue if it doesn't exist
        string queueUrl = await sqsClient.GetOrCreateQueueUrlAsync(queueName);
        string queueArn = await sqsClient.GetQueueArnAsync(queueUrl);

        // Allow SNS to send to SQS (Policy)
        await sqsClient.SetSnsToSqsPolicyAsync(topicArn, queueUrl, queueArn);
        // Subscribe SQS to SNS topic
        await snsClient.AllowSNSToSendToSQSAsync(topicArn, queueArn);

        using var mongoClient = new MongoClient(MONGODB_ENDPOINT);
        var database = mongoClient.GetDatabase(dbName);                 
        IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(collectionName);

        await collection.StartListenToChangeStreamAsync(async change =>
        {
            var request = new PublishRequest
            {
                TopicArn = topicArn,
                Message = change
            };
            await snsClient.PublishAsync(request);
        }, cancellationToken);
    }

    public static async Task ListenToOubox  (this QueueSinkSetting setting, CancellationToken cancellationToken)
    {
        (string dbName, string collectionName, string queueName) = setting;

        using var sqsClient = AwsProviderFactory.CreateSqsClient();

        // Create SQS queue if it doesn't exist
        string queueUrl = await sqsClient.GetOrCreateQueueUrlAsync(queueName);

        using var mongoClient = new MongoClient(MONGODB_ENDPOINT);
        var database = mongoClient.GetDatabase(dbName);                 
        IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(collectionName);

        await collection.StartListenToChangeStreamAsync(async change =>
        {
            var request = new SendMessageRequest
            {
                QueueUrl = queueUrl,
                MessageBody = change
            };
            await sqsClient.SendMessageAsync(request);
        }, cancellationToken);
    }

    #endregion //  OuboxTo
}
