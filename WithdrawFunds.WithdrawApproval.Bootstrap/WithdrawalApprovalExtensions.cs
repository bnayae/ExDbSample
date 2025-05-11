using Funds.Withdraw.RequestWithdrawFundsViaATM;
using Funds.Withdraw.WithdrawFunds;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions;

public static class FetchFundsExtensions
{
    public static IServiceCollection AddWithdrawalApprovalConsumer(this IServiceCollection services)
    {
        services.TryAddWithdrawalApprovalSlice();
        services.TryAddWithdrawApprovalBridge();
        services.AddBridgedSQSProcessor<FundsWithdrawalRequestedViaAtmMessage, WithdrawalApprovalRequest>("WithdrawApprover");

        return services;
    }
}


