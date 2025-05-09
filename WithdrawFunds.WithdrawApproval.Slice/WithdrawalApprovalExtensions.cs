using Core.Abstractions;
using Funds.Withdraw.WithdrawFunds;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions;

public static class WithdrawalApprovalExtensions
{
    public static IServiceCollection TryAddWithdrawalApprovalSlice(this IServiceCollection services)
    {
        services.TryAddSingleton<ICommandHandler<WithdrawalApprovalRequest>, WithdrawalApproval>();
        return services;
    }
}