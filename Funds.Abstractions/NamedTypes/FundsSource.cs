using Vogen;

namespace Funds.Abstractions;

#pragma warning disable S1210 // "Equals" and the comparison operators should be overridden when implementing "IComparable"
[ValueObject<string>(Conversions.TypeConverter | Conversions.SystemTextJson,
    toPrimitiveCasting: CastOperator.Implicit,
    fromPrimitiveCasting: CastOperator.Implicit,
    tryFromGeneration: TryFromGeneration.GenerateBoolMethod,
    isInitializedMethodGeneration: IsInitializedMethodGeneration.Generate)]
public readonly partial struct FundsSource
{
    private static string NormalizeInput(string input) => input.Trim();

}
