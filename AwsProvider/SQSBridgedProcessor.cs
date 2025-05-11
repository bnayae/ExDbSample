using Core.Abstractions;
using EvDb.Core;
#pragma warning disable S101 // Types should be named in PascalCase


namespace Microsoft.Extensions;

/// <summary>
/// Indirect (bridged) SQS processor.
/// </summary>
/// <typeparam name="TMessage"></typeparam>
/// <typeparam name="TRequest"></typeparam>
internal class SQSBridgedProcessor<TMessage, TRequest> : SQSProcessorBase<TMessage>
{
    /// <summary>
    /// Bridges the message to a command request and processes it.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="bridge"></param>
    /// <param name="commandHandler"></param>
    /// <param name="filter">Filter function to determine if the message should be processed.</param>
    /// <param name="queueName"></param>
    public SQSBridgedProcessor(ILogger<SQSBridgedProcessor<TMessage, TRequest>> logger,
                        IProcessorToCommandBridge<TMessage, TRequest> bridge,
                        ICommandHandler<TRequest> commandHandler,
                        Func<IEvDbMessageMeta, bool> filter,
                        string queueName) :
                            base(logger, (m, ct) => MessageBridgingAsync(bridge, commandHandler, m, ct), filter, queueName)
    {
    }

    /// <summary>
    /// Bridges the message to a command request and processes it.
    /// </summary>
    /// <param name="bridge"></param>
    /// <param name="commandHandler"></param>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private static async Task MessageBridgingAsync(IProcessorToCommandBridge<TMessage, TRequest> bridge,
                                             ICommandHandler<TRequest> commandHandler,
                                             TMessage message,
                                             CancellationToken cancellationToken)
    {
        var request = await bridge.BridgeAsync(message, cancellationToken);
        await commandHandler.ExecuteAsync(request, cancellationToken);
    }
}
