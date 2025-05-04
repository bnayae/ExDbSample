using Funds.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Routing;

using Vogen;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Funds.Withdraw.RequestWithdrawFundsViaATM;

namespace Microsoft.Extensions;

public static class FetchFundsExtensions
{
    public static IEndpointRouteBuilder AddRequestWithdrawFundsViaATM(this IEndpointRouteBuilder app)
    {
        var withdraw = app.MapGroup("withdraw")
         .WithTags("Withdraw");

        withdraw.MapPost("ATM/{account}", 
            async (AccountId account, 
            FundsTransactionData data,
            IFetchFundsFromAtm slice) =>
        {
            FetchFundsFromAtmRequest request = new(account, data);
            await slice.ProcessAsync(request);
            return Results.Ok();
        });
        return app;
    }
}


