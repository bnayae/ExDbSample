﻿using System.Collections.Immutable;

namespace Funds.Withdraw.WithdrawFunds;

[EvDbViewType<ImmutableArray<WithdrawalsInProcessItem>, IWithdrawFundsRequestEvents>("withdrawals-in-inprocess")]
public partial class WithdrawalsInProcessView
{
    protected override ImmutableArray<WithdrawalsInProcessItem> DefaultState { get; } = ImmutableArray<WithdrawalsInProcessItem>.Empty;

    public override int MinEventsBetweenSnapshots { get; } = 10;

    protected override ImmutableArray<WithdrawalsInProcessItem> Apply(ImmutableArray<WithdrawalsInProcessItem> state, FundsWithdrawalApprovedEvent payload, IEvDbEventMeta meta)
    {
        var item = new WithdrawalsInProcessItem(payload.AccountId, payload.Data);
        state = state.Add(item);
        return state;
    }
}
