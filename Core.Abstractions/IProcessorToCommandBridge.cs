namespace Core.Abstractions;

/// <summary>
/// Bridge between a message consumption to a command's request.
/// </summary>
/// <typeparam name="TMessage">The type of the message.</typeparam>
/// <typeparam name="TRequest">The type of the command's request.</typeparam>
public interface IProcessorToCommandBridge<in TMessage, TRequest>
{
    /// <summary>
    /// Bridge between a message consumption to a command's request.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<TRequest> BridgeAsync(TMessage request, CancellationToken cancellationToken = default);
}