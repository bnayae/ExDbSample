using Core.Abstractions;
using EvDb.Core;
#pragma warning disable S101 // Types should be named in PascalCase

namespace Microsoft.Extensions;

/// <summary>
/// Direct SQS processor.
/// </summary>
/// <typeparam name="T"></typeparam>
internal class SQSDirectProcessor<T> : SQSProcessorBase<T>
{
    /// <summary>
    /// Direct SQS processor constructor.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="processor">The processor logic</param>
    /// <param name="commandHandler"></param>
    /// <param name="filter">Filter function to determine if the message should be processed.</param>
    /// <param name="queueName"></param>
    public SQSDirectProcessor(ILogger<SQSDirectProcessor<T>> logger,
                        IProcessorToCommand<T>? processor,
                        ICommandHandler<T> commandHandler,
                        Func<IEvDbMessageMeta, bool> filter,
                        string queueName) : base(logger,
                                                 (m, ct) => ExecuteAsync(processor, commandHandler, m, ct),
                                                 filter,
                                                 queueName)
    {
    }

    /// <summary>
    /// Bridges the message to a command request and processes it.
    /// </summary>
    /// <param name="processor"></param>
    /// <param name="commandHandler"></param>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private static async Task ExecuteAsync(IProcessorToCommand<T>? processor,
                                             ICommandHandler<T> commandHandler,
                                             T message,
                                             CancellationToken cancellationToken)
    {
        if(processor != null)
            message = await processor.ProcessAsync(message, cancellationToken);
        await commandHandler.ExecuteAsync(message, cancellationToken);
    }

}