namespace Funds.Withdraw.WithdrawFunds;

// @Roma: naming duplication ["Funds.Withdraw", "Withdraw.Funds.Request"]


/// <summary>
/// Withdraw Funds Request factory.
/// </summary>
[EvDbStreamFactory<IWithdrawFundsRequestEvents, WithdrawFundsRequestOutbox>("Funds.Withdraw", "Withdraw.Funds.Request")]
public partial class WithdrawFundsRequestFactory
{
}
