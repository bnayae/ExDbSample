// Ignore Spelling: sns
// Ignore Spelling: sqs

using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using Core.Abstractions;
using EvDb.Core;
using Microsoft.Extensions.DependencyInjection;
#pragma warning disable S101 // Types should be named in PascalCase
#pragma warning disable CA1303 // Do not pass literals as localized parameters

namespace Microsoft.Extensions;

public static class AWSProviderExtensions
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
            Console.WriteLine($"SQS queue: {queueUrl} created");
        }
        return queueUrl;
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

    /// <summary>
    /// Adds the SQS processor.
    /// This method registers a hosted service that processes messages from an SQS queue into a command.
    /// </summary>
    /// <typeparam name="T">The type of both the SQS message and the command's request.</typeparam>
    /// <param name="services">The services.</param>
    /// <param name="queueName">Name of the queue.</param>
    /// <returns></returns>
    public static IServiceCollection AddDirectSQSProcessor<T>(this IServiceCollection services, string queueName)
    {
        services.AddHostedService(sp =>
        {
            ICommandHandler<T> commandEntry = sp.GetRequiredService<ICommandHandler<T>>();
            var logger = sp.GetRequiredService<ILogger<SQSDirectProcessor<T>>>();
            var host = new SQSDirectProcessor<T>(
                logger,
                commandEntry,
                null,
                queueName);
            return host;
        });
        return services;
    }

    /// <summary>
    /// Adds the SQS processor.
    /// This method registers a hosted service that processes messages from an SQS queue into a command.
    /// </summary>
    /// <typeparam name="T">The type of both the SQS message and the command's request.</typeparam>
    /// <param name="services">The services.</param>
    /// <param name="filter">Filter function to determine if the message should be processed.</param>
    /// <param name="queueName">Name of the queue.</param>
    /// <returns></returns>
    public static IServiceCollection AddDirectSQSProcessor<T>(this IServiceCollection services, Func<IEvDbMessageMeta, bool> filter, string queueName)
    {
        services.AddHostedService(sp =>
        {
            ICommandHandler<T> commandEntry = sp.GetRequiredService<ICommandHandler<T>>();
            var logger = sp.GetRequiredService<ILogger<SQSDirectProcessor<T>>>();
            var host = new SQSDirectProcessor<T>(
                logger,
                commandEntry,
                filter,
                queueName);
            return host;
        });
        return services;
    }

    #endregion //  AddDirectSQSProcessor

    #region AddBridgedSQSProcessor

    /// <summary>
    /// Adds the SQS processor.
    /// This method registers a hosted service that processes messages from an SQS queue into a command.
    /// </summary>
    /// <typeparam name="TMessage">The type of both the SQS message.</typeparam>
    /// <typeparam name="TRequest">The type of both the command's request.</typeparam>
    /// <param name="services">The services.</param>
    /// <param name="queueName">Name of the queue.</param>
    /// <returns></returns>
    public static IServiceCollection AddBridgedSQSProcessor<TMessage, TRequest>(
                                            this IServiceCollection services,
                                            string queueName)
    {
        services.AddHostedService(sp =>
        {
            ICommandHandler<TRequest> commandEntry = sp.GetRequiredService<ICommandHandler<TRequest>>();
            IProcessorToCommandBridge<TMessage, TRequest> bridge = sp.GetRequiredService<IProcessorToCommandBridge<TMessage, TRequest>>();
            var logger = sp.GetRequiredService<ILogger<SQSBridgedProcessor<TMessage, TRequest>>>();
            var host = new SQSBridgedProcessor<TMessage, TRequest>(
                logger,
                bridge,
                commandEntry,
                null,
                queueName);
            return host;
        });
        return services;
    }

    /// <summary>
    /// Adds the SQS processor.
    /// This method registers a hosted service that processes messages from an SQS queue into a command.
    /// </summary>
    /// <typeparam name="TMessage">The type of both the SQS message.</typeparam>
    /// <typeparam name="TRequest">The type of both the command's request.</typeparam>
    /// <param name="services">The services.</param>
    /// <param name="filter">Filter function to determine if the message should be processed.</param>
    /// <param name="queueName">Name of the queue.</param>
    /// <returns></returns>
    public static IServiceCollection AddBridgedSQSProcessor<TMessage, TRequest>(
                                            this IServiceCollection services,
                                            Func<IEvDbMessageMeta, bool> filter,
                                            string queueName)
    {
        services.AddHostedService(sp =>
        {
            ICommandHandler<TRequest> commandEntry = sp.GetRequiredService<ICommandHandler<TRequest>>();
            IProcessorToCommandBridge<TMessage, TRequest> bridge = sp.GetRequiredService<IProcessorToCommandBridge<TMessage, TRequest>>();
            var logger = sp.GetRequiredService<ILogger<SQSBridgedProcessor<TMessage, TRequest>>>();
            var host = new SQSBridgedProcessor<TMessage, TRequest>(
                logger,
                bridge,
                commandEntry,
                filter,
                queueName);
            return host;
        });
        return services;
    }

    #endregion //  AddBridgedSQSProcessor
}
