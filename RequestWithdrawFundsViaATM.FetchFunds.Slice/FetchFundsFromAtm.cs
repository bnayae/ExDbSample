using Core.Abstractions;
using Funds.Abstractions;

namespace Funds.Withdraw.RequestWithdrawFundsViaATM;

internal sealed class FetchFundsFromAtm : ICommandHandler<FetchFundsFromAtmRequest>
{
    private const string INITIATE_METHOD = "ATM";

    private readonly ILogger<FetchFundsFromAtm> _logger;
    private readonly IEvDbAtmFundsWithdrawFactory _fundsFactory;

    public FetchFundsFromAtm(
        ILogger<FetchFundsFromAtm> logger,
        IEvDbAtmFundsWithdrawFactory fundsFactory)
    {
        _logger = logger;
        _fundsFactory = fundsFactory;
    }

    async Task ICommandHandler<FetchFundsFromAtmRequest>.ProcessAsync(FetchFundsFromAtmRequest request,
                                        CancellationToken cancellationToken)
    {
        (AccountId account, FundsTransactionData data) = request;

        IEvDbAtmFundsWithdraw stream = await _fundsFactory.GetAsync(account, cancellationToken);

        if (request.Data.Amount < 1000)
        {
            var e = new FundsFetchRequestedFromAtmEvent(account, data, INITIATE_METHOD);
            await stream.AddAsync(e);
        }
        else
        {
            var e = new FundsFetchRequestedFromAtmDeniedEvent
            {
                AccountId = account,
                Data = data,
                InitiateMethod = INITIATE_METHOD
            };
            await stream.AddAsync(e);
        }

        StreamStoreAffected response = await stream.StoreAsync(cancellationToken);
        _logger.LogFetchFundsFromAtm(account, data, response);
    }
}
