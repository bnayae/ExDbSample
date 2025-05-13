using Funds.Abstractions;

namespace Funds.Withdraw.WithdrawFunds;

/// <summary>
/// Found to withdraw message
/// </summary>
/// <param name="AccountId"></param>
/// <param name="Data"></param>
/// <param name="InitiateMethod"></param>
/// <param name="Commission"></param>
[EvDbAttachChannel(WithdrawFundsChannels.Todo)]
[EvDbDefineMessagePayload("funds-to-withdraw")]
public readonly partial record struct FundsToWithdrawMessage(AccountId AccountId,
                                              FundsTransactionData Data,
                                              FundsInitiateMethod InitiateMethod,
                                              Commission Commission);
