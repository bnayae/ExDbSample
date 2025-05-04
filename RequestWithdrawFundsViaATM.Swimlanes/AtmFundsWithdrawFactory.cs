namespace Funds.Withdraw.RequestWithdrawFundsViaATM;

/// <summary>
/// ATM's funds withdrawal factory.
/// </summary>
[EvDbStreamFactory<IAtmFundsEvents, AtmFundsOutbox>("Funds.Withdraw", "ATM.Funds")]
public partial class AtmFundsWithdrawFactory
{
}
