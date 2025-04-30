using Funds.Abstractions;

namespace Funds.Withdraw.ATM;

public readonly record struct FetchFundsFromAtmRequest(Guid Account,
                                                       FundsTransactionData Data);
