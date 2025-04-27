using EvDbSample.Repositories;

namespace EvDbSample.Repositories;


[EvDbStreamFactory<IAtmFundsEvents, AtmFundsOutbox>("Funds", "ATM")]
public partial class AtmFundsFactory
{
}
