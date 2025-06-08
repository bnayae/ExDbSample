using Funds.Abstractions;
using System.Collections.Immutable;

namespace Funds.Withdraw.WithdrawFunds;

[EvDbViewType<ImmutableDictionary<Currency, AccountBalanceState>, IAccountFundsEvents>("withdrawals-in-inprocess")]
public partial class AccountBalanceView
{
    protected override ImmutableDictionary<Currency, AccountBalanceState> DefaultState { get; } = ImmutableDictionary<Currency, AccountBalanceState>.Empty;

    public override int MinEventsBetweenSnapshots { get; } = 10;


    protected override ImmutableDictionary<Currency, AccountBalanceState> Apply(ImmutableDictionary<Currency, AccountBalanceState> state, FundsWithdrawnFromAccountEvent payload, IEvDbEventMeta meta)
    {
        return base.Apply(state, payload, meta);
    }
}
