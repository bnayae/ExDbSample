using Funds.Abstractions;

namespace Funds.Withdraw.ATM;

[EvDbDefineEventPayload("funds-fetch-requested-from-atm")]
public readonly partial record struct FundsFetchRequestedFromATM(
                                            FundsTransactionData Data)
{
}
