using EvDb.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using static Funds.Abstractions.FundsConstants.Swimlanes;
// Ignore Spelling: Swimlanes

namespace Microsoft.Extensions;

public static class WithdrawFundsSwimlanesExtensions
{
    public static IHostApplicationBuilder AddWithdrawFundsSwimlanes(this IHostApplicationBuilder builder)
    {
        IServiceCollection services = builder.Services;

        services.AddEvDb()
        .AddAccountFundsFactory(storage => storage.UseMongoDBStoreForEvDbStream(MONGODbConnectionKey),
                                    new EvDbStorageContext(DatabaseName,
                                                                builder.Environment.EnvironmentName,
                                                                "Account.Funds"))
        .DefaultSnapshotConfiguration(storage => storage.UseMongoDBForEvDbSnapshot(MONGODbConnectionKey));

        services.AddEvDb()
                .AddWithdrawFundsRequestFactory(storage => storage.UseMongoDBStoreForEvDbStream(MONGODbConnectionKey),
                                            new EvDbStorageContext(DatabaseName,
                                                                        builder.Environment.EnvironmentName,
                                                                        "Withdraw.Funds.Request"))
                .DefaultSnapshotConfiguration(storage => storage.UseMongoDBForEvDbSnapshot(MONGODbConnectionKey));

        return builder;
    }
}


