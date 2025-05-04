using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funds.Withdraw.WithdrawFunds;

internal static partial class Logs
{
    [LoggerMessage(LogLevel.Debug, "Withdrawal approved for account {AccountId}")]
    public static partial void WithdrawalApproved(this ILogger logger, Guid accountId);

    [LoggerMessage(LogLevel.Debug, "Withdrawal denied for account {AccountId}")]
    public static partial void WithdrawalDenied(this ILogger logger, Guid accountId);
}
