using EvDbSample.Abstractions;
using EvDbSample.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvDbSample.ATM.Services;

internal class AtmFundsService
{
    private readonly ILogger<AtmFundsService> _logger;
    private readonly IEvDbAtmFundsFactory _fundsFactory;

    public AtmFundsService(
        ILogger<AtmFundsService> logger,
        IEvDbAtmFundsFactory fundsFactory)
    {
        _logger = logger;
        _fundsFactory = fundsFactory;
    }

    public async Task DepositFundsAsync(string account,
                                        FundsDepositedData data,
                                        CancellationToken cancellationToken = default)
    {
        IEvDbAtmFunds stream = await _fundsFactory.GetAsync(account, cancellationToken);
        AtmFundsDepositedEvent e = new (data);
        await stream.AddAsync(e); 
        await stream.StoreAsync(cancellationToken);
    }
}
