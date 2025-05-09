namespace Core.Abstractions;

/// <summary>
/// Command contract
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ICommandHandler<in T>
{
    /// <summary>
    /// Command entry.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task ProcessAsync(T request, CancellationToken cancellationToken = default);
}
