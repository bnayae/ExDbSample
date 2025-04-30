using Funds.Abstractions;

namespace Funds.Withdraw.ATM;

internal sealed class FetchFundsFromAtm : IFetchFundsFromAtm
{
    private const string CONTEXT = "withdraw";

    private readonly ILogger<FetchFundsFromAtm> _logger;
    private readonly IEvDbAtmFundsWithdrawFactory _fundsFactory;

    public FetchFundsFromAtm(
        ILogger<FetchFundsFromAtm> logger,
        IEvDbAtmFundsWithdrawFactory fundsFactory)
    {
        _logger = logger;
        _fundsFactory = fundsFactory;
    }

    public async Task ProcessAsync(FetchFundsFromAtmRequest request,
                                        CancellationToken cancellationToken = default)
    {
        (Guid account, FundsTransactionData data) = request;

        IEvDbAtmFundsWithdraw stream = await _fundsFactory.GetAsync(account, cancellationToken);
        data = data with { FlowContexts = data.FlowContexts.Add(CONTEXT) };
        FundsFetchRequestedFromATM e = new(data);
        await stream.AddAsync(e);
        StreamStoreAffected response = await stream.StoreAsync(cancellationToken);
        _logger.LogFetchFundsFromAtm(account, data, response);
    }
}
