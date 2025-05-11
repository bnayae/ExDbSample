using Core.Abstractions;
using Funds.Withdraw.WithdrawFunds;
using Funds.Withdraw.WithdrawFunds.WithdrawApproval.Slice;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions;

public static class CommissionCalculationExtensions
{
    public static IServiceCollection TryCommissionCalculationCommand(this IServiceCollection services)
    {
        services.TryAddSingleton<ICommandHandler<CalculateWithdrawCommissionRequest>, CalculateWithdrawCommissionCommand>();
        return services;
    }
}