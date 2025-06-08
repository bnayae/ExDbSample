using Core.Abstractions;
using Funds.Abstractions;

namespace Funds.Withdraw.RequestWithdrawFundsViaATM;

internal sealed class FetchFundsFromAtmCommand : ICommandHandler<FetchFundsFromAtmRequest>
{
    private const string INITIATE_METHOD = "ATM";

    private readonly ILogger<FetchFundsFromAtmCommand> _logger;
    private readonly IEvDbAtmFundsWithdrawFactory _fundsFactory;

    public FetchFundsFromAtmCommand(
        ILogger<FetchFundsFromAtmCommand> logger,
        IEvDbAtmFundsWithdrawFactory fundsFactory)
    {
        _logger = logger;
        _fundsFactory = fundsFactory;
    }

    async Task ICommandHandler<FetchFundsFromAtmRequest>.ExecuteAsync(FetchFundsFromAtmRequest request,
                                        CancellationToken cancellationToken)
    {
        (AccountId account, FundsTransactionData data) = request;

        IEvDbAtmFundsWithdraw stream = await _fundsFactory.GetAsync(account, cancellationToken);

        if (request.Data.Amount < 1000)
        {
            var e = new FundsFetchRequestedFromAtmEvent(account, data, INITIATE_METHOD);
            await stream.AppendAsync(e);
        }
        else
        {
            var e = new FundsFetchRequestedFromAtmDeniedEvent
            {
                AccountId = account,
                Data = data,
                InitiateMethod = INITIATE_METHOD
            };
            await stream.AppendAsync(e);
        }

        StreamStoreAffected response = await stream.StoreAsync(cancellationToken);
        _logger.LogFetchFundsFromAtm(account, data, response);
    }
}
