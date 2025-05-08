using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SQS;

namespace Microsoft.Extensions;

public static class AwsProviderFactory
{
    private readonly static BasicAWSCredentials CREDENTIALS = new BasicAWSCredentials("test", "test");
    private const string AWS_ENDPOINT = "http://localhost:4566";
    private const string REGION = "us-east-1";

    #region CreateSnsClient

    public static AmazonSimpleNotificationServiceClient CreateSnsClient()
    {
        var config = new AmazonSimpleNotificationServiceConfig
        {
            ServiceURL = AWS_ENDPOINT,
            AuthenticationRegion = REGION
        };
        return new AmazonSimpleNotificationServiceClient(CREDENTIALS, config);
    }

    #endregion //  CreateSnsClient

    #region CreateSqsClient

    public static AmazonSQSClient CreateSqsClient()
    {
        var config = new AmazonSQSConfig
        {
            ServiceURL = AWS_ENDPOINT,
            AuthenticationRegion = REGION
        };
        return new AmazonSQSClient(CREDENTIALS, config);
    }

    #endregion //  CreateSqsClient
}
