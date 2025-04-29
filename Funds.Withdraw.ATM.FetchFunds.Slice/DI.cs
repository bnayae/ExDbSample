using Funds.Withdraw.ATM;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions;

public static class FetchFundsSliceDI
{
    public static IServiceCollection AddFetchFundsSlice(this IServiceCollection services)
    {
        services.AddScoped<IFetchFundsFromAtm, FetchFundsFromAtm>();
        return services;
    }
}
