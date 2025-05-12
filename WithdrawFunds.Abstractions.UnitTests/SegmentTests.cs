using Funds.Withdraw.WithdrawFunds;

namespace WithdrawFunds.Abstractions.UnitTests;

public class SegmentTests
{    
    [Theory]
    [InlineData("foo", "foo")]
    [InlineData(" foo", "foo")]
    [InlineData("foo ", "foo")]
    public void SegmentValidationTest(string value, string expected)
    {
        Assert.Equal(expected, Segment.From(value));
    }
}