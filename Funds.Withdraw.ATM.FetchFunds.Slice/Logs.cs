using Funds.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funds.Withdraw.ATM;

public static partial class Logs
{
    [LoggerMessage(LogLevel.Debug, "Fetch Funds From ATM Started {account}, {data}")]
    public static partial void LogFetchFundsFromAtmStarted(this ILogger logger,
                                                           string account,
                                                           [LogProperties] FundsTransactionData data);
}
