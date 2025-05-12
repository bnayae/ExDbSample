using Vogen;

#pragma warning disable S1210 // "Equals" and the comparison operators should be overridden when implementing "IComparable"
namespace Funds.Abstractions;

/// <summary>
/// The method of initiating the funds operation (Like ATM, Teller, PayPal, etc.)
/// </summary>
[ValueObject<string>(Conversions.TypeConverter | Conversions.SystemTextJson,
    toPrimitiveCasting: CastOperator.Implicit,
    fromPrimitiveCasting: CastOperator.Implicit,
    tryFromGeneration: TryFromGeneration.GenerateBoolMethod,
    isInitializedMethodGeneration: IsInitializedMethodGeneration.Generate)]
public readonly partial struct FundsInitiateMethod
{
    private static Validation Validate(string input) => input switch
    { 
        null => Validation.Invalid("Cannot be null"),
        { Length: < 2 } => Validation.Invalid("Expecting at least 2 Letters"),
        _ => Validation.Ok
    };

    private static string NormalizeInput(string input) => input.Trim();

}
