using Funds.Withdraw.Abstractions;

namespace Funds.Withdraw.ATM;

/// <summary>
/// Produce outbox messages form ATM funds withdrawal.
/// </summary>
[EvDbAttachMessageType<FundsWithdrawalRequested>]
[EvDbOutbox<AtmFundsWithdrawFactory>]
internal sealed partial class AtmFundsOutbox
{
    protected override void ProduceOutboxMessages(FundsFetchRequestedFromATM payload,
                                                  IEvDbEventMeta meta,
                                                  EvDbAtmFundsWithdrawViews views,
                                                  AtmFundsOutboxContext outbox)
    {
        // Create a command for the withdrawal flow
        FundsWithdrawalRequested msg = new(payload.Data);
        outbox.Add(msg);
    }
}
