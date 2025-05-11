namespace Core.Abstractions;

/// <summary>
/// Inject processor logic between a message consumption to a command's invocation (request_.
/// </summary>
/// <typeparam name="T">The type of both the message and the request.</typeparam>
public interface IProcessorToCommand<T>
{
    /// <summary>
    /// The processor logic between a message consumption to a command's invocation.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<T> ProcessAsync(T request, CancellationToken cancellationToken = default);
}