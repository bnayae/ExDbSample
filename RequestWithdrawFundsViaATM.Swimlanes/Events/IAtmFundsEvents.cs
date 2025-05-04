namespace Funds.Withdraw.RequestWithdrawFundsViaATM;

[EvDbAttachEventType<FundsFetchRequestedFromAtmEvent>]
[EvDbAttachEventType<FundsFetchRequestedFromAtmDeniedEvent>]
public partial interface IAtmFundsEvents
{
}
