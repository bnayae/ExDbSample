
using System.Collections.Immutable;
using EvDbSample.Abstractions;

namespace EvDbSample.Repositories;

[EvDbDefineEventPayload("funds-deposited")]
public readonly partial record struct FundsDepositRequestedEvent(
                                        FundsDepositedData Data, 
                                        DepositSource Source)
{
    public required ImmutableArray<string> Segments { get; init; } = [];
}
