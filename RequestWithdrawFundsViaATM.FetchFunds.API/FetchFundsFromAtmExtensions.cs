using Funds.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Routing;

using Vogen;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Funds.Withdraw.RequestWithdrawFundsViaATM;
using Core.Abstractions;

namespace Microsoft.Extensions;

public static class FetchFundsFromAtmApiExtensions
{
    public static IEndpointRouteBuilder AddRequestWithdrawFundsViaATM(this IEndpointRouteBuilder app)
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


