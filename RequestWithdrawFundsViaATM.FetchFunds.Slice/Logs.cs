using Funds.Abstractions;

namespace Funds.Withdraw.RequestWithdrawFundsViaATM;

internal static partial class Logs
{
    [LoggerMessage(LogLevel.Debug, "Fetch Funds From ATM Started {account}, {data}, {response}")]
    public static partial void LogFetchFundsFromAtm(this ILogger logger,
                                                           Guid account,
                                                           [LogProperties] FundsTransactionData data,
                                                           [LogProperties] StreamStoreAffected response);
}
