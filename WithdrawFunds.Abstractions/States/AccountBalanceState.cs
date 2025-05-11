using Funds.Abstractions;

namespace Funds.Withdraw.WithdrawFunds;


public readonly record struct AccountBalanceState(AccountId AccountId,
                                                  Currency Currency,
                                                  double Funds,
                                                  DateTimeOffset ApprovalDate);
