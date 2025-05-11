using Funds.Abstractions;

namespace Funds.Withdraw.WithdrawFunds;

[EvDbDefineEventPayload("withdraw-commission-calculated")]
public readonly partial record struct WithdrawCommissionCalculatedEvent(AccountId AccountId,
                                              FundsTransactionData Data,
                                              FundsInitiateMethod InitiateMethod,
                                              Commission Commission);
