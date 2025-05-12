using Funds.Abstractions;
using System.Globalization;

namespace WithdrawFunds.Abstractions.UnitTests;

public class CurrencyTests
{    
    [Theory]
    [InlineData("atm", true)]
    [InlineData(" atm", true)]
    public void CurrencyValidationTest(string value, bool succeed)
    {
        Assert.Equal(succeed, Currency.TryFrom(value, out Currency result));
        if(succeed)
            Assert.Equal(value.Trim().ToUpper(CultureInfo.InvariantCulture), result.Value);
    }
}