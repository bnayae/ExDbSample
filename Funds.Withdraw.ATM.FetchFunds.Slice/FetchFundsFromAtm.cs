using Funds.Abstractions;

namespace Funds.Withdraw.ATM;

internal class FetchFundsFromAtm
{
    private readonly ILogger<FetchFundsFromAtm> _logger;
    private readonly IEvDbAtmFundsWithdrawFactory _fundsFactory;

    public FetchFundsFromAtm(
        ILogger<FetchFundsFromAtm> logger,
        IEvDbAtmFundsWithdrawFactory fundsFactory)
    {
        _logger = logger;
        _fundsFactory = fundsFactory;
    }

    public async Task DepositFundsAsync(string account,
                                        FundsTransactionData data,
                                        CancellationToken cancellationToken = default)
    {
        IEvDbAtmFundsWithdraw stream = await _fundsFactory.GetAsync(account, cancellationToken);
        FundsFetchRequestedFromATM e = new(data);
        await stream.AddAsync(e);
        await stream.StoreAsync(cancellationToken);
    }
}
