using EvDb.Core;
using Funds.Abstractions;
using Funds.Withdraw.WithdrawFunds;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions;

public static class CommissionCalculationExtensions
{
    public static IServiceCollection AddCommissionCalculationConsumer(
                                    this IServiceCollection services,
                                    Func<IEvDbMessageMeta, bool>? filter = null)
    {
        services.TryCommissionCalculationCommand();
        services.TryAddCommissionCalculationProcessor();
        services.AddBridgedSQSProcessor<CalculateWithdrawalsCommissionMessage, CalculateWithdrawCommissionRequest>(
                                                                                        filter ?? (_ => true), 
                                                                                        FundsConstants.Queues.CalculateWithdrawalsCommission);

        return services;
    }

}
