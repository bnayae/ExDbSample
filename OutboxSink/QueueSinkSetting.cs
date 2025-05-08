namespace Microsoft.Extensions;

public readonly record struct QueueSinkSetting(string DbName, string CollectionName, string QueueName);
