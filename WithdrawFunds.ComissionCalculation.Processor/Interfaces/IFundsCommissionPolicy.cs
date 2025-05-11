using System.Collections.Immutable;

namespace Funds.Withdraw.WithdrawFunds;

/// <summary>
/// Commission policy contract
/// </summary>
public interface IFundsCommissionPolicy
{
    /// <summary>
    /// Gets the commission for a given account and flow context.
    /// In for of a dictionary when the key is a segment and the commission represented as a double between 0 and 1
    /// </summary>
    /// <param name="segments">Segmentations</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<Commission> GetCommissionAsync(
        ImmutableArray<Segment> segments,
        CancellationToken cancellationToken = default);
}
