using Core.Abstractions;
using Funds.Withdraw.RequestWithdrawFundsViaATM;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions;

public static class FetchFundsSliceExtensions
{
    public static IServiceCollection TryAddFetchFundsSlice(this IServiceCollection services)
    {
        services.TryAddSingleton<ICommandHandler<FetchFundsFromAtmRequest>, FetchFundsFromAtm>();
        return services;
    }
}
