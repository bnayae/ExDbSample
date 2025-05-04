using Funds.Abstractions;

namespace Funds.Withdraw.RequestWithdrawFundsViaATM;

/// <summary>
/// Withdrawal requested message via ATM
/// </summary>
/// <param name="AccountId">Account identifier</param>
/// <param name="Data">Common transaction data</param>
/// <param name="InitiateMethod">The method of initiating the funds operation (like ATM, Teller, PayPal, etc.)</param>
[EvDbDefineMessagePayload("funds-deposited-requested-via-atm")]
public readonly partial record struct FundsWithdrawalRequestedViaAtmMessage(AccountId AccountId,
                                                      FundsTransactionData Data,
                                                      FundsInitiateMethod InitiateMethod);