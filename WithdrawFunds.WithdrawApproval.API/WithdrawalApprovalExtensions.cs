using Funds.Withdraw.WithdrawFunds;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions;

public static class FetchFundsExtensions
{
    // TODO: Get Lookups

    public static IServiceCollection AddWithdrawalApprovalConsumer(this IServiceCollection services)
    {
        services.TryAddWithdrawalApprovalSlice();
        services.AddSqsProcessor<WithdrawalApprovalRequest>("WithdrawApprover");

        return services;
    }
}


