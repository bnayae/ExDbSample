using Funds.Withdraw.ATM;

namespace Funds.Withdraw.Repositories;

[EvDbAttachEventType<FundsFetchRequestedFromATM>]
public partial interface IAtmFundsEvents
{
}
