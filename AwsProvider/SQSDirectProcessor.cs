using Core.Abstractions;
#pragma warning disable S101 // Types should be named in PascalCase

namespace Microsoft.Extensions;

/// <summary>
/// Direct SQS processor.
/// </summary>
/// <typeparam name="T"></typeparam>
internal class SQSDirectProcessor<T> : SQSProcessorBase<T>
{
    public SQSDirectProcessor(ILogger<SQSDirectProcessor<T>> logger,
                        ICommandHandler<T> commandHandler,
                        string queueName) : base(logger, commandHandler.ProcessAsync, queueName)
    {
    }
}