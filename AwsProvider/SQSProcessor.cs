using Core.Abstractions;
using EvDb.Core;
#pragma warning disable S101 // Types should be named in PascalCase


namespace Microsoft.Extensions;


/// <summary>
/// Direct SQS processor.
/// </summary>
/// <typeparam name="T"></typeparam>
internal class SQSProcessor<T> : SQSProcessorBase<T>
{
    /// <summary>
    /// SQS processor constructor.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="processor">The processor logic</param>
    /// <param name="commandHandler">The target command handler</param>
    /// <param name="filter">Filter function to determine if the message should be processed.</param>
    /// <param name="queueName">The queue name for consumption</param>
    /// <param name="visibilityTimeout">The SQS message visibility timeout</param>
    public SQSProcessor(ILogger<SQSProcessor<T>> logger,
                        IProcessor<T>? processor,
                        ICommandHandler<T> commandHandler,
                        Func<IEvDbMessageMeta, bool> filter,
                        string queueName,
                        TimeSpan visibilityTimeout) : base(logger,
                                                 (m, ct) => ExecuteAsync(processor, commandHandler, m, ct),
                                                 filter,
                                                 queueName,
                                                 visibilityTimeout)
    {
    }

    /// <summary>
    /// Handle the processor logic.
    /// </summary>
    /// <param name="processor"></param>
    /// <param name="commandHandler">The target command handler</param>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private static async Task ExecuteAsync(IProcessor<T>? processor,
                                             ICommandHandler<T> commandHandler,
                                             T message,
                                             CancellationToken cancellationToken)
    {
        if (processor != null)
            message = await processor.ProcessAsync(message, cancellationToken);
        await commandHandler.ExecuteAsync(message, cancellationToken);
    }

}

/// <summary>
/// Indirect (bridged) SQS processor.
/// </summary>
/// <typeparam name="TMessage"></typeparam>
/// <typeparam name="TRequest"></typeparam>
internal class SQSProcessor<TMessage, TRequest> : SQSProcessorBase<TMessage>
{
    /// <summary>
    /// Bridges the message to a command request and processes it.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="bridge">Map message to request and handle the processor logic</param>
    /// <param name="commandHandler">The target command handler</param>
    /// <param name="filter">Filter function to determine if the message should be processed.</param>
    /// <param name="queueName">The queue name for consumption</param>
    /// <param name="visibilityTimeout">The SQS message visibility timeout</param>
    public SQSProcessor(ILogger<SQSProcessor<TMessage, TRequest>> logger,
                        IProcessor<TMessage, TRequest> bridge,
                        ICommandHandler<TRequest> commandHandler,
                        Func<IEvDbMessageMeta, bool> filter,
                        string queueName,
                        TimeSpan visibilityTimeout) :
                            base(logger,
                                 (m, ct) => MessageBridgingAsync(bridge, commandHandler, m, ct),
                                 filter,
                                 queueName,
                                 visibilityTimeout)
    {
    }

    /// <summary>
    /// Bridges the message to a command request and processes it.
    /// </summary>
    /// <param name="bridge">Map message to request and handle the processor logic</param>
    /// <param name="commandHandler">The target command handler</param>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private static async Task MessageBridgingAsync(IProcessor<TMessage, TRequest> bridge,
                                             ICommandHandler<TRequest> commandHandler,
                                             TMessage message,
                                             CancellationToken cancellationToken)
    {
        var request = await bridge.ProcessAsync(message, cancellationToken);
        await commandHandler.ExecuteAsync(request, cancellationToken);
    }
}
