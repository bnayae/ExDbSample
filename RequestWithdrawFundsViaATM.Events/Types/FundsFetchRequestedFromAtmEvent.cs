
namespace Funds.Withdraw.RequestWithdrawFundsViaATM;

/// <summary>
/// Funds fetch requested event via ATM
/// </summary>
/// <param name="AccountId">Account identifier</param>
/// <param name="Data">Common transaction data</param>
/// <param name="InitiateMethod">The method of initiating the funds operation (like ATM, Teller, PayPal, etc.)</param>
[EvDbDefineEventPayload("funds-fetch-requested-from-ATM")]
public readonly partial record struct FundsFetchRequestedFromAtmEvent(AccountId AccountId,
                                                      FundsTransactionData Data,
                                                      FundsInitiateMethod InitiateMethod);
