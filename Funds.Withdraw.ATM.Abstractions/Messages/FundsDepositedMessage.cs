
using System.Collections.Immutable;

namespace EvDbSample.Abstractions;

[EvDbDefineMessagePayload("funds-deposited")]
public readonly partial record struct FundsDepositedMessage(FundsDepositedData Data,
                                                            DepositSource Source)
{
    public required ImmutableArray<string> Segments { get; init; } = [];
}
