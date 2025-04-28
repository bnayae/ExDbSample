using Funds.Withdraw.Repositories;

namespace Funds.Withdraw.ATM;

/// <summary>
/// ATM's funds withdrawal factory.
/// </summary>
[EvDbStreamFactory<IAtmFundsEvents, AtmFundsOutbox>("Funds", "ATM-Withdraw")]
public partial class AtmFundsWithdrawFactory
{
}
