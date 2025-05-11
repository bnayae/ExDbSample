using Core.Abstractions;
using Funds.Withdraw.WithdrawFunds;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions;

public static class WithdrawalApprovalExtensions
{
    public static IServiceCollection TryAddWithdrawalApprovalCommand(this IServiceCollection services)
    {
        services.TryAddSingleton<ICommandHandler<WithdrawalApprovalRequest>, WithdrawalApprovalCommand>();
        return services;
    }
}