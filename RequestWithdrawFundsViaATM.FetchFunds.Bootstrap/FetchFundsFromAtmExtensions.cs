using Core.Abstractions;
using Funds.Abstractions;
using Funds.Withdraw.RequestWithdrawFundsViaATM;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions;

public static class FetchFundsFromAtmApiExtensions
{
    public static IServiceCollection AddRequestWithdrawFundsViaATM(this IServiceCollection services)
    {
        services.TryAddFetchFundsSlice();
        return services;
    }

    public static IEndpointRouteBuilder UseRequestWithdrawFundsViaATM(this IEndpointRouteBuilder app)
    {
        var withdraw = app.MapGroup("withdraw")
         .WithTags("Withdraw");

        withdraw.MapPost("ATM/{account}",
            async (AccountId account,
            FundsTransactionData data,
            ICommandHandler<FetchFundsFromAtmRequest> slice) =>
        {
            FetchFundsFromAtmRequest request = new(account, data);
            await slice.ProcessAsync(request);
            return Results.Ok();
        });
        return app;
    }
}


