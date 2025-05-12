using Funds.Abstractions;

namespace WithdrawFunds.Abstractions;

public class AccountIdTests
{    
    [Fact]
    public void AccountIdValidationTest()
    {
        Guid guid = Guid.NewGuid();
        AccountId accountId = guid;
        Assert.Equal(guid, accountId.Value);
    }
}