// Ignore Spelling: sns
// Ignore Spelling: sqs

using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using Core.Abstractions;
using EvDb.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
#pragma warning disable S101 // Types should be named in PascalCase
#pragma warning disable CA1303 // Do not pass literals as localized parameters

namespace Microsoft.Extensions;

public static class AWSProviderExtensions
{
    private static readonly SemaphoreSlim _streamLock = new(1, 1);
    private static readonly SemaphoreSlim _queueLock = new(1, 1);

    #region GetOrCreateStreamAsync

    public static async Task<string> GetOrCreateStreamAsync(this AmazonSimpleNotificationServiceClient snsClient, string topicName)
    {
        await _streamLock.WaitAsync(6000);
        try
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
        finally
        {
            _streamLock.Release();
        }
    }

    #endregion //  GetOrCreateStreamAsync

    #region GetOrCreateQueueUrlAsync

    public static async Task<string> GetOrCreateQueueUrlAsync(this AmazonSQSClient sqsClient,
                                                              string queueName,
                                                              TimeSpan visibilityTimeout)
    {
        await _queueLock.WaitAsync(6000);
        try
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
                string visibilityValue =  ((int)visibilityTimeout.TotalSeconds).ToString(CultureInfo.InvariantCulture);
                await sqsClient.SetQueueAttributesAsync(new SetQueueAttributesRequest
                {
                    QueueUrl = queueUrl,
                    Attributes = new Dictionary<string, string>
                                        {
                                            { QueueAttributeName.VisibilityTimeout, visibilityValue}
                                        }
                });

                Console.WriteLine($"SQS queue: {queueUrl} created");
            }
            return queueUrl;
        }
        finally
        {
            _queueLock.Release();
        }
    }

    #endregion //  GetOrCreateQueueUrlAsync

    #region GetQueueArnAsync

#pragma warning disable CA1062 // Validate arguments of public methods
#pragma warning disable CA1054 // URI-like parameters should not be strings
#pragma warning restore CA1054 // URI-like parameters should not be strings
#pragma warning disable CA1054 // URI-like parameters should not be strings
    public static async Task<string> GetQueueARNAsync(this AmazonSQSClient sqsClient, string queueUrl)
#pragma warning restore CA1054 // URI-like parameters should not be strings
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

    #endregion // GetQueueARNAsync

    #region SetSNSToSQSPolicyAsync

#pragma warning disable CA1054 // URI-like parameters should not be strings
    /// <summary>
    /// Allow SNS to send to SQS (Policy)
    /// </summary>
    /// <param name="sqsClient">The SQS client.</param>
    /// <param name="topicARN">The topic ARN.</param>
    /// <param name="queueURL">The queue URL.</param>
    /// <param name="queueARN">The queue ARN.</param>
    /// <returns></returns>
    public static async Task SetSNSToSQSPolicyAsync(this AmazonSQSClient sqsClient,
                                                    string topicARN,
                                                    string queueURL,
                                                    string queueARN)
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
      "Resource":"{{queueARN}}",
      "Condition":{
        "ArnEquals":{"aws:SourceArn":"{{topicARN}}"}
      }
    }
    ]
    }
    """;

        await sqsClient.SetQueueAttributesAsync(queueURL, new Dictionary<string, string>
        {
            { "Policy", policy }
        });
    }
#pragma warning restore CA1054 // URI-like parameters should not be strings

    #endregion //  SetSNSToSQSPolicyAsync

    #region AllowSNSToSendToSQSAsync
#pragma warning disable CA1031 // Do not catch general exception types
#pragma warning restore CA1062 // Validate arguments of public methods

    public static async Task AllowSNSToSendToSQSAsync(
                            this AmazonSimpleNotificationServiceClient snsClient,
                            string topicARN,
                            string queueARN)
    {
        // Check if subscription exists, if not create it
        var snsSubscriptions = await snsClient.ListSubscriptionsByTopicAsync(topicARN);
        Console.WriteLine($"Found {snsSubscriptions.Subscriptions?.Count ?? 0} subscriptions for SNS topic");

        bool subscriptionExists = false;
        foreach (var sub in snsSubscriptions.Subscriptions ?? [])
        {
            Console.WriteLine($"  - {sub.Protocol}: {sub.Endpoint}");
            if (sub.Protocol == "sqs" && sub.Endpoint.Contains(queueARN, StringComparison.OrdinalIgnoreCase))
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
                    TopicArn = topicARN,
                    Protocol = "sqs",
                    Endpoint = queueARN
                });
                Console.WriteLine($"Created subscription: {subscribeResponse.SubscriptionArn}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating subscription: {ex.Message}");
            }
        }
    }
#pragma warning restore CA1031 // Do not catch general exception types

    #endregion //  AllowSNSToSendToSQSAsync

    #region AddDirectSQSProcessor

    #region Overloads

    /// <summary>
    /// Adds the SQS processor.
    /// This method registers a hosted service that processes messages from an SQS queue into a command.
    /// </summary>
    /// <typeparam name="T">The type of both the SQS message and the command's request.</typeparam>
    /// <param name="services">The services.</param>
    /// <param name="queueName">Name of the queue.</param>
    /// <param name="visibilityTimeout">The SQS message visibility timeout</param>
    /// <returns></returns>
    public static IServiceCollection AddDirectSQSProcessor<T>(this IServiceCollection services, string queueName, TimeSpan visibilityTimeout)
    {
        return services.AddDirectSQSProcessor<T>(_ => true, queueName, visibilityTimeout);
    }

    #endregion //  Overloads

    /// <summary>
    /// Adds the SQS processor.
    /// This method registers a hosted service that processes messages from an SQS queue into a command.
    /// </summary>
    /// <typeparam name="T">The type of both the SQS message and the command's request.</typeparam>
    /// <param name="services">The services.</param>
    /// <param name="filter">Filter function to determine if the message should be processed.</param>
    /// <param name="queueName">Name of the queue.</param>
    /// <param name="visibilityTimeout">The SQS message visibility timeout</param>
    /// <returns></returns>
    public static IServiceCollection AddDirectSQSProcessor<T>(this IServiceCollection services,
                                                              Func<IEvDbMessageMeta, bool> filter,
                                                              string queueName,
                                                              TimeSpan visibilityTimeout)
    {
        services.AddHostedService(sp =>
        {
            ICommandHandler<T> commandEntry = sp.GetRequiredService<ICommandHandler<T>>();
            IProcessor<T>? processor = sp.GetService<IProcessor<T>>();
            var logger = sp.GetRequiredService<ILogger<SQSProcessor<T>>>();
            var host = new SQSProcessor<T>(
                logger,
                processor,
                commandEntry,
                filter,
                queueName,
                visibilityTimeout);
            return host;
        });
        return services;
    }

    #endregion //  AddDirectSQSProcessor

    // TODO: [bnaya 2025-06-08] used keyed registration to specific stream `sp.GetRequiredKeyedService<IProcessor<TMessage, TRequest>>(key)`

    #region AddSQSProcessor

    #region Overloads

    /// <summary>
    /// Adds the SQS processor from event stream bridge.
    /// This method registers a hosted service that processes messages from an SQS queue from event stream bridge into a command.
    /// </summary>
    /// <typeparam name="TMessage">The type of both the SQS message.</typeparam>
    /// <typeparam name="TRequest">The type of both the command's request.</typeparam>
    /// <param name="services">The services.</param>
    /// <param name="queueName">Name of the queue.</param>
    /// <param name="visibilityTimeout">The SQS message visibility timeout</param>
    /// <returns></returns>
    public static IServiceCollection AddBridgedSQSProcessor<TMessage, TRequest>(
                                            this IServiceCollection services,
                                            string queueName,
                                            TimeSpan visibilityTimeout)
    {
        return services.AddBridgedSQSProcessor<TMessage, TRequest>(_ => true, queueName, visibilityTimeout);
    }

    #endregion //  Overloads

    /// <summary>   
    /// Adds the SQS processor from event stream bridge.
    /// This method registers a hosted service that processes messages from an SQS queue from event stream bridge into a command.
    /// </summary>
    /// <typeparam name="TMessage">The type of both the SQS message.</typeparam>
    /// <typeparam name="TRequest">The type of both the command's request.</typeparam>
    /// <param name="services">The services.</param>
    /// <param name="filter">Filter function to determine if the message should be processed.</param>
    /// <param name="consumeFromQueueName">Name of the queue (consume from).</param>
    /// <param name="visibilityTimeout">The SQS message visibility timeout</param>
    /// <returns></returns>
    public static IServiceCollection AddBridgedSQSProcessor<TMessage, TRequest>(
                                                this IServiceCollection services,
                                                Func<IEvDbMessageMeta, bool> filter,
                                                string consumeFromQueueName,
                                                TimeSpan visibilityTimeout)
    {
        services.AddHostedService(sp =>
        {
            ICommandHandler<TRequest> commandEntry = sp.GetRequiredService<ICommandHandler<TRequest>>();
            IProcessor<TMessage, TRequest> bridge = sp.GetRequiredService<IProcessor<TMessage, TRequest>>();
            var logger = sp.GetRequiredService<ILogger<SQSProcessor<TMessage, TRequest>>>();
            var host = new SQSProcessor<TMessage, TRequest>(
                logger,
                bridge,
                commandEntry,
                filter,
                consumeFromQueueName,
                visibilityTimeout);
            return host;
        });
        return services;
    }

    #endregion //  AddDirectSQSProcessor
}
