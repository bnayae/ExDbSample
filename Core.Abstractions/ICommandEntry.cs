namespace Core.Abstractions;

public interface ICommandEntry<T>
{
    /// <summary>
    /// Command entry.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task ProcessAsync(T request, CancellationToken cancellationToken = default);
}