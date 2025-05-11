namespace Funds.Withdraw.WithdrawFunds;

internal static partial class Logs
{
    [LoggerMessage(LogLevel.Debug, "Withdrawal commission calculated for account {AccountId}")]
    public static partial void WithdrawCommissionCalculated(this ILogger logger, Guid accountId);

}
