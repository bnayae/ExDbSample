using Funds.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funds.Withdraw.WithdrawFunds;

internal class WithdrawalApproval: IApproveWithdrawal
{
    private readonly ILogger<WithdrawalApproval> _logger;

    public WithdrawalApproval(ILogger<WithdrawalApproval> logger)
	{
        _logger = logger;
    }

    async Task IApproveWithdrawal.ProcessAsync(WithdrawalApprovalRequest request,
                                               CancellationToken cancellationToken)
    {
        (AccountId account, FundsTransactionData data, FundsInitiateMethod initiateMethod, double effectiveAccountBalance) = request;

        if (effectiveAccountBalance < data.Amount)
        {

            _logger.WithdrawalApproved(account);
        }
        else
        {
            _logger.WithdrawalDenied(request.AccountId);
        }
        throw new NotImplementedException();
    }
}

