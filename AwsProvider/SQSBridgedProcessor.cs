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
    public SQSBridgedProcessor(ILogger<SQSBridgedProcessor<TMessage, TRequest>> logger,
                        IProcessorToCommandBridge<TMessage, TRequest> bridge,
                        ICommandHandler<TRequest> commandHandler,
                        Func<EvDbMessage, bool>? filter,
                        string queueName) : 
                            base(logger, (m,ct) => MessageBridging(bridge, commandHandler, m, ct), filter, queueName)
    {
    }

    private static async Task MessageBridging(IProcessorToCommandBridge<TMessage, TRequest> bridge, 
                                             ICommandHandler<TRequest> commandHandler,
                                             TMessage message,
                                             CancellationToken cancellationToken)
    {
        var request = await bridge.BridgeAsync(message, cancellationToken);
        await commandHandler.ProcessAsync(request, cancellationToken);
    }
}
