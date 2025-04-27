
using System.Collections.Immutable;
using EvDbSample.Abstractions;

namespace EvDbSample.Repositories;

[EvDbDefineEventPayload("ATM-funds-deposited")]
public readonly partial record struct AtmFundsDepositedEvent(FundsDepositedData Data)
{
}
