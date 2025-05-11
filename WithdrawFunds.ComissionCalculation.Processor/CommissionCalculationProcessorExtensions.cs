using Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Funds.Withdraw.WithdrawFunds;

public static class CommissionCalculationProcessorExtensions
{
    public static IServiceCollection TryAddCommissionCalculationProcessor(this IServiceCollection services)
    {
        services.TryAddSingleton<IProcessorToCommandBridge<CalculateWithdrawalsCommissionMessage, CalculateWithdrawCommissionRequest>, CommissionCalculationProcessor>();
        return services;
    }

}
