using Vogen;

#pragma warning disable S1210 // "Equals" and the comparison operators should be overridden when implementing "IComparable"
namespace Funds.Abstractions;

/// <summary>
/// Describe the high level flow context unit
/// </summary>
[ValueObject<string>(Conversions.TypeConverter | Conversions.SystemTextJson,
    toPrimitiveCasting: CastOperator.Implicit,
    fromPrimitiveCasting: CastOperator.Implicit,
    tryFromGeneration: TryFromGeneration.GenerateBoolMethod,
    isInitializedMethodGeneration: IsInitializedMethodGeneration.Generate)]
public readonly partial struct FundsFlowContext
{
    private static string NormalizeInput(string input) => input.Trim();

}
