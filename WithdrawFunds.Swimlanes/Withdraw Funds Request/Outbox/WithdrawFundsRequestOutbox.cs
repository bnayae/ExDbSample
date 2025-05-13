namespace Funds.Withdraw.WithdrawFunds;

[EvDbAttachMessageType<CalculateWithdrawalsCommissionMessage>]
[EvDbAttachMessageType<FundsToWithdrawMessage>]
[EvDbOutbox<WithdrawFundsRequestFactory>]
internal partial class WithdrawFundsRequestOutbox
{
    #region void ProduceOutboxMessages(FundsWithdrawalApprovedEvent payload,...)

    protected override void ProduceOutboxMessages(FundsWithdrawalApprovedEvent payload,
                                                  IEvDbEventMeta meta,
                                                  EvDbWithdrawFundsRequestViews views,
                                                  WithdrawFundsRequestOutboxContext outbox)
    {
        var message = new CalculateWithdrawalsCommissionMessage
        {
            AccountId = payload.AccountId,
            Data = payload.Data,
            InitiateMethod = payload.InitiateMethod
        };
        outbox.Add(message);
    }

    #endregion //  void ProduceOutboxMessages(FundsWithdrawalApprovedEvent payload,...)

    #region void ProduceOutboxMessages(WithdrawCommissionCalculatedEvent payload, ...)

    protected override void ProduceOutboxMessages(WithdrawCommissionCalculatedEvent payload, IEvDbEventMeta meta, EvDbWithdrawFundsRequestViews views, WithdrawFundsRequestOutboxContext outbox)
    {
        var message = new FundsToWithdrawMessage
        {
            AccountId = payload.AccountId,
            Data = payload.Data,
            InitiateMethod = payload.InitiateMethod,
            Commission = payload.Commission
        };
        outbox.Add(message);
    }

    #endregion //  void ProduceOutboxMessages(WithdrawCommissionCalculatedEvent payload, ...)
}
