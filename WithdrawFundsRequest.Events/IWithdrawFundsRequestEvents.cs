namespace Funds.Withdraw.WithdrawFunds;

[EvDbAttachEventType<FundsWithdrawalApprovedEvent>]
[EvDbAttachEventType<FundsWithdrawalDeclinedEvent>]
[EvDbAttachEventType<WithdrawCommissionCalculatedEvent>]
public partial interface IWithdrawFundsRequestEvents
{
}
