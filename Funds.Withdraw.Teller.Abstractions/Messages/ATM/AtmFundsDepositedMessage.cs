
using System.Collections.Immutable;

namespace EvDbSample.Abstractions;

[EvDbAttachChannel("ATM")]
[EvDbDefineMessagePayload("funds-deposit-requested")]
public readonly partial record struct AtmFundsDepositedMessage(FundsDepositedData Data)
{
}
