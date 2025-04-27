using EvDbSample.Repositories;

namespace EvDbSample.Repositories;


[EvDbStreamFactory<IFundsEvents, FundsOutbox>("Funds", "common")]
public partial class FundsFactory
{
}
