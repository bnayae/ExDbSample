namespace Funds.Withdraw.RequestWithdrawFundsViaATM;

/// <summary>
/// Fetch funds from ATM.
/// </summary>
public interface IFetchFundsFromAtm
{
    /// <summary>
    /// Processes the asynchronous.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    Task ProcessAsync(FetchFundsFromAtmRequest request, CancellationToken cancellationToken = default);
}
