using Funds.Abstractions;

namespace Funds.Withdraw.RequestWithdrawFundsViaATM;

/// <summary>
/// Fetch funds from ATM request
/// </summary>
/// <param name="AccountId">AccountId identifier</param>
/// <param name="Data">Common transaction data</param>
public readonly record struct FetchFundsFromAtmRequest(AccountId AccountId,
                                                       FundsTransactionData Data);
