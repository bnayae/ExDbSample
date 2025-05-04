using Funds.Abstractions;
using System.Collections.Immutable;

namespace Funds.Withdraw.WithdrawFunds;

/// <summary>
/// Segmentation policy contract
/// </summary>
public interface IFundsSegmentation
{
    /// <summary>
    /// Gets the segments of an account.
    /// </summary>
    /// <param name="accountId">The account identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task<ImmutableArray<Segment>> GetSegmentsAsync(
        AccountId accountId,
        CancellationToken cancellationToken = default);
}
