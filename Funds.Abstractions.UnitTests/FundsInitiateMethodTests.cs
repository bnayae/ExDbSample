using Funds.Abstractions;
using System.Globalization;

namespace WithdrawFunds.Abstractions.UnitTests;

public class FundsInitiateMethodTests
{    
    [Theory]
    [InlineData("At", true)]
    [InlineData("Atm", true)]
    [InlineData(" atm", true)]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("x", false)]
    public void FundsInitiateMethodValidationTest(string value, bool succeed)
    {
        Assert.Equal(succeed, FundsInitiateMethod.TryFrom(value, out FundsInitiateMethod result));
        if(succeed)
            Assert.Equal(value.Trim(), result.Value);
    }
}