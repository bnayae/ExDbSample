namespace Core.Abstractions;

/// <summary>
/// Bridge between a message consumption to a command's message,
/// And add processor logic between a message consumption to a command's invocation.
/// </summary>
/// <typeparam name="TMessage">The type of the message.</typeparam>
/// <typeparam name="TRequest">The type of the command's message.</typeparam>
public interface IProcessorToCommandBridge<in TMessage, TRequest>
{
    /// <summary>
    /// Bridge between a message consumption to a command's message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<TRequest> BridgeAsync(TMessage message, CancellationToken cancellationToken = default);
}
