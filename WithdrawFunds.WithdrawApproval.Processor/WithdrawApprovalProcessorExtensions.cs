using Core.Abstractions;
using Funds.Withdraw.RequestWithdrawFundsViaATM;
using Funds.Withdraw.WithdrawFunds;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WithdrawFunds.WithdrawApproval.Processor;

namespace Microsoft.Extensions;

public static class WithdrawApprovalProcessorExtensions
{
    public static IServiceCollection TryAddWithdrawApprovalProcessor(this IServiceCollection services)
    {
        services.TryAddSingleton<IProcessor<FundsWithdrawalRequestedViaAtmMessage, WithdrawalApprovalRequest>, WithdrawApprovalProcessor>();
        return services;
    }
}