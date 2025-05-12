using Funds.Withdraw.WithdrawFunds;

namespace WithdrawFunds.Abstractions.UnitTests;

public class CommissionTests
{    
    [Theory]
    [InlineData(0.0, true)]
    [InlineData(1, true)]
    [InlineData(0.5, true)]
    [InlineData(1.5, false)]
    [InlineData(-0.5, false)]
    public void CommissionValidationTest(double value, bool expected)
    {
        Assert.Equal(expected, Commission.TryFrom(value, out Commission result));
        if(expected)
            Assert.Equal(value, result.Value);
    }
}