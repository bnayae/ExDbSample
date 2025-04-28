using Funds.Abstractions;

namespace Funds.Withdraw.Abstractions;

[EvDbDefineMessagePayload("funds-deposited-requested")]
public readonly partial record struct FundsWithdrawalRequested(FundsTransactionData Data)
{
}
