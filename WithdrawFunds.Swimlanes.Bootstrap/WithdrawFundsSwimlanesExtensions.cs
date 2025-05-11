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
        .AddAccountFundsFactory(storage => storage.UseMongoDBStoreForEvDbStream(MONGO_CONNECTION_KEY),
                                    new EvDbStorageContext(DATABASE,
                                                                builder.Environment.EnvironmentName,
                                                                "AccountFunds"))
        .DefaultSnapshotConfiguration(storage => storage.UseMongoDBForEvDbSnapshot(MONGO_CONNECTION_KEY));

        services.AddEvDb()
                .AddWithdrawFundsRequestFactory(storage => storage.UseMongoDBStoreForEvDbStream(MONGO_CONNECTION_KEY),
                                            new EvDbStorageContext(DATABASE,
                                                                        builder.Environment.EnvironmentName,
                                                                        "WithdrawFundsRequest"))
                .DefaultSnapshotConfiguration(storage => storage.UseMongoDBForEvDbSnapshot(MONGO_CONNECTION_KEY));

        return builder;
    }
}


