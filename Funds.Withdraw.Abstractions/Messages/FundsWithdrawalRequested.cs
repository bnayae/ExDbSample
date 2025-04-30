using Funds.Abstractions;

namespace Funds.Withdraw.Abstractions;

/// <summary>
/// Withdrawal requested message
/// </summary>
/// <param name="Data">Common transaction data</param>
/// <param name="InitiateMethod">The method of initiating the funds operation</param>
[EvDbDefineMessagePayload("funds-deposited-requested")]
public readonly partial record struct FundsWithdrawalRequested(
                                                FundsTransactionData Data,
                                                FundsInitiateMethod InitiateMethod)
{
}
