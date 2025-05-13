namespace Core.Abstractions;

/// <summary>
/// Inject processor logic between a message consumption to a command's invocation (request_.
/// </summary>
/// <typeparam name="T">The type of both the message and the request.</typeparam>
public interface IProcessor<T>
{
    /// <summary>
    /// The processor logic between a message consumption to a command's invocation.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<T> ProcessAsync(T request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Bridge between a message consumption to a command's message,
/// And add processor logic between a message consumption to a command's invocation.
/// </summary>
/// <typeparam name="TMessage">The type of the message.</typeparam>
/// <typeparam name="TRequest">The type of the command's message.</typeparam>
public interface IProcessor<in TMessage, TRequest>
{
    /// <summary>
    /// Bridge between a message consumption to a command's message 
    /// and handle the processing logic.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<TRequest> ProcessAsync(TMessage message, CancellationToken cancellationToken = default);
}
