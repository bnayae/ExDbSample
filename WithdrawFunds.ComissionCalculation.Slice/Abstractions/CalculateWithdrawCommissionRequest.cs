using Funds.Abstractions;

namespace Funds.Withdraw.WithdrawFunds;

/// <summary>
/// Withdrawal approval request
/// </summary>
/// <param name="AccountId">Account identifier</param>
/// <param name="Data">Common transaction data</param>
/// <param name="InitiateMethod">The method of initiating the funds operation (like ATM, Teller, PayPal, etc.)</param>
/// <param name="Commission">The Commission</param>
public record CalculateWithdrawCommissionRequest(AccountId AccountId,
                                        FundsTransactionData Data,
                                        FundsInitiateMethod InitiateMethod,
                                        Commission Commission);