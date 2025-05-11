using EvDb.Core;
using Funds.Withdraw.RequestWithdrawFundsViaATM;
using Funds.Withdraw.WithdrawFunds;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions;

public static class FetchFundsExtensions
{
    public static IServiceCollection AddWithdrawalApprovalConsumer(
                                        this IServiceCollection services,
                                        Func<IEvDbMessageMeta, bool>? filter = null)
    {
        services.TryAddWithdrawalApprovalCommand();
        services.TryAddWithdrawApprovalProcessor();
        services.AddBridgedSQSProcessor<FundsWithdrawalRequestedViaAtmMessage, WithdrawalApprovalRequest>(filter ?? (_ => true), "WithdrawApprover");

        return services;
    }
}


