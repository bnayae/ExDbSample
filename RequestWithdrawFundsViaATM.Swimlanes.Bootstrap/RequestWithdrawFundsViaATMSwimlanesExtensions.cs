// Ignore Spelling: Swimlanes

using EvDb.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using static Funds.Abstractions.FundsConstants.Swimlanes;

namespace Microsoft.Extensions;

public static class RequestWithdrawFundsViaATMSwimlanesExtensions
{
    public static IHostApplicationBuilder AddRequestWithdrawFundsViaATMSwimlanes(this IHostApplicationBuilder builder)
    {
        IServiceCollection services = builder.Services;
        services.AddEvDb()
                .AddAtmFundsWithdrawFactory(storage => storage.UseMongoDBStoreForEvDbStream(MONGODbConnectionKey),
                                            new EvDbStorageContext(DatabaseName,
                                                                        builder.Environment.EnvironmentName,
                                                                        "ATM.Funds"))
                .DefaultSnapshotConfiguration(storage => storage.UseMongoDBForEvDbSnapshot(MONGODbConnectionKey));

        return builder;
    }
}


