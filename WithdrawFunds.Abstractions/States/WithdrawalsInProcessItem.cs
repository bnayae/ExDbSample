using Funds.Abstractions;

namespace Funds.Withdraw.WithdrawFunds;


public readonly record struct WithdrawalsInProcessItem(AccountId AccountId,
                                                      FundsTransactionData Data);
