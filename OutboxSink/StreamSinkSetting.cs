namespace Microsoft.Extensions;

public readonly record struct StreamSinkSetting(string DbName, string CollectionName, string StreamName, string QueueName);
