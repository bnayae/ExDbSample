using Core.Abstractions;
using Funds.Withdraw.RequestWithdrawFundsViaATM;
using Funds.Withdraw.WithdrawFunds;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WithdrawFunds.WithdrawApproval.Processor;

namespace Microsoft.Extensions;

public static class WithdrawApprovalBridgeExtensions
{
    public static IServiceCollection TryAddWithdrawApprovalBridge(this IServiceCollection services)
    {
        services.TryAddSingleton<IProcessorToCommandBridge<FundsWithdrawalRequestedViaAtmMessage, WithdrawalApprovalRequest>, WithdrawApprovalBridge>();
        return services;
    }
}