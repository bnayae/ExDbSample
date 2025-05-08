namespace Funds.Withdraw.RequestWithdrawFundsViaATM;

/// <summary>
/// Produce outbox messages form ATM funds withdrawal.
/// </summary>
[EvDbAttachMessageType<FundsWithdrawalRequestedViaAtmMessage>]
[EvDbOutbox<AtmFundsWithdrawFactory>("ReactToWithdrawalRequestedViaATM")]
internal sealed partial class AtmFundsOutbox
{
    protected override void ProduceOutboxMessages(FundsFetchRequestedFromAtmEvent payload,
                                                  IEvDbEventMeta meta,
                                                  EvDbAtmFundsWithdrawViews views,
                                                  AtmFundsOutboxContext outbox)
    {
        // Create a command for the withdrawal flow
        var msg = new FundsWithdrawalRequestedViaAtmMessage
        {
            AccountId = payload.AccountId,
            Data = payload.Data,
            InitiateMethod = payload.InitiateMethod
        };
        outbox.Add(msg);
    }
}
