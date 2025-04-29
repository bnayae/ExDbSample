using Funds.Abstractions;

namespace Funds.Withdraw.ATM;

public readonly record struct FetchFundsFromAtmRequest(string Account,
                                                       FundsTransactionData data);
