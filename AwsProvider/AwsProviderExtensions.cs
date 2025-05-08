using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;


namespace Microsoft.Extensions;

public static class AwsProviderExtensions
{
    #region GetOrCreateTopicAsync

    public static async Task<string> GetOrCreateTopicAsync(this AmazonSimpleNotificationServiceClient snsClient, string topicName)
    {
        var listTopicsResponse = await snsClient.ListTopicsAsync();
        string? topicArn = listTopicsResponse.Topics switch
        {
            null => null,
            { Count: > 0 } => listTopicsResponse.Topics[0].TopicArn,
            _ => null
        };


        if (string.IsNullOrEmpty(topicArn))
        {
            Console.WriteLine($"SNS topic not found, creating {topicName}...");
            var createTopicResponse = await snsClient.CreateTopicAsync(topicName);
            topicArn = createTopicResponse.TopicArn;
            Console.WriteLine($"SNS topic: {topicArn} created");
        }
        else
        {
            Console.WriteLine($"Using existing SNS topic: {topicArn}");
        }

        return topicArn;
    }

    #endregion //  GetOrCreateTopicAsync

    #region GetOrCreateQueueUrlAsync

    public static async Task<string> GetOrCreateQueueUrlAsync(this AmazonSQSClient sqsClient, string queueName)
    {
        var listQueuesResponse = await sqsClient.ListQueuesAsync(new ListQueuesRequest
        {
            QueueNamePrefix = queueName
        });

        string queueUrl;
        string? existingQueueUrl = null;
        if (listQueuesResponse.QueueUrls is not null)
        {
            existingQueueUrl = listQueuesResponse.QueueUrls
                                                    .FirstOrDefault(url => url.EndsWith($"/{queueName}",
                                                                         StringComparison.OrdinalIgnoreCase));
        }

        if (existingQueueUrl is not null)
        {
            queueUrl = existingQueueUrl;
            Console.WriteLine($"Found existing SQS queue: {queueUrl}");
        }
        else
        {
            var createQueueResponse = await sqsClient.CreateQueueAsync(queueName);
            queueUrl = createQueueResponse.QueueUrl;
            Console.WriteLine($"SQS queue: {queueUrl} created);
        }
        return queueUrl;
    }

    #endregion //  GetOrCreateQueueUrlAsync

    #region GetQueueArnAsync

    public static async Task<string> GetQueueArnAsync(this AmazonSQSClient sqsClient, string queueUrl)
    {
        var attrs = await sqsClient.GetQueueAttributesAsync(new GetQueueAttributesRequest
        {
            QueueUrl = queueUrl,
            AttributeNames = new List<string> { "QueueArn" }
        });
        string arn = attrs.Attributes["QueueArn"];
        Console.WriteLine($"Queue ARN: {arn}");
        return arn;
    }

    #endregion //  GetQueueArnAsync

    #region SetSnsToSqsPolicyAsync


    /// <summary>
    /// Allow SNS to send to SQS (Policy)
    /// </summary>
    /// <param name="sqsClient">The SQS client.</param>
    /// <param name="topicArn">The topic arn.</param>
    /// <param name="queueUrl">The queue URL.</param>
    /// <returns></returns>
    public static async Task SetSnsToSqsPolicyAsync(this AmazonSQSClient sqsClient, string topicArn, string queueUrl, string queueArn)
    {
        string policy = $$"""
{
  "Version":"2012-10-17",
  "Statement":[
    {
      "Sid":"AllowSNSPublish",
      "Effect":"Allow",
      "Principal":{"AWS":"*"},
      "Action":"sqs:SendMessage",
      "Resource":"{{queueArn}}",
      "Condition":{
        "ArnEquals":{"aws:SourceArn":"{{topicArn}}"}
      }
    }
  ]
}
""";

        await sqsClient.SetQueueAttributesAsync(queueUrl, new Dictionary<string, string>
{
    { "Policy", policy }
});
        await sqsClient.SetQueueAttributesAsync(queueUrl, new Dictionary<string, string>
        {
            { "Policy", policy }
        });
    }

    #endregion //  SetSnsToSqsPolicyAsync

    #region AllowSNSToSendToSQSAsync

    public static async Task AllowSNSToSendToSQSAsync(
                            this AmazonSimpleNotificationServiceClient snsClient,
                            string topicArn,
                            string queueArn)
    {
        // Check if subscription exists, if not create it
        var snsSubscriptions = await snsClient.ListSubscriptionsByTopicAsync(topicArn);
        Console.WriteLine($"Found {snsSubscriptions.Subscriptions?.Count ?? 0} subscriptions for SNS topic");

        bool subscriptionExists = false;
        foreach (var sub in snsSubscriptions.Subscriptions ?? [])
        {
            Console.WriteLine($"  - {sub.Protocol}: {sub.Endpoint}");
            if (sub.Protocol == "sqs" && sub.Endpoint.Contains(queueArn))
            {
                subscriptionExists = true;
            }
        }

        if (!subscriptionExists)
        {
            Console.WriteLine("Creating SNS to SQS subscription...");
            try
            {
                var subscribeResponse = await snsClient.SubscribeAsync(new SubscribeRequest
                {
                    TopicArn = topicArn,
                    Protocol = "sqs",
                    Endpoint = queueArn
                });
                Console.WriteLine($"Created subscription: {subscribeResponse.SubscriptionArn}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating subscription: {ex.Message}");
            }
        }
    }

    #endregion //  AllowSNSToSendToSQSAsync

    #region StartListenToChangeStreamAsync

    public static async Task StartListenToChangeStreamAsync(this IMongoCollection<BsonDocument> collection,
                                                            Func<string, Task> onChange,
                                                            CancellationToken cancellationToken)
    {
        using var changeStream = await collection.WatchAsync();

        while (!cancellationToken.IsCancellationRequested && await changeStream.MoveNextAsync())
        {
            foreach (var change in changeStream.Current)
            {
                //var json = change.FullDocument.ToJson();
                var message = change.FullDocument.ToJson() ?? "MISSING";
                await onChange(message);
            }
        }
    }

    #endregion //  StartListenToChangeStreamAsync

    #region AddSqsProcessor

    public static IServiceCollection AddSqsProcessor<T>(this IServiceCollection services, string queueName)
    {
        services.AddHostedService(sp =>
        {
            ICommandEntry<T> commandEntry = sp.GetRequiredService<ICommandEntry<T>>();
            var logger = sp.GetRequiredService<ILogger<SqsProcessor<T>>>();
            var host = new SqsProcessor<T>(
                logger,
                commandEntry,
                queueName);
            return host;
        });
        return services;
    }

    #endregion //  AddSqsProcessor
}
