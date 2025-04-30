using Funds.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funds.Withdraw.ATM;

public static partial class Logs
{
    [LoggerMessage(LogLevel.Debug, "Fetch Funds From ATM Started {account}, {data}, {response}")]
    public static partial void LogFetchFundsFromAtm(this ILogger logger,
                                                           Guid account,
                                                           [LogProperties] FundsTransactionData data,
                                                           [LogProperties] StreamStoreAffected response);
}
