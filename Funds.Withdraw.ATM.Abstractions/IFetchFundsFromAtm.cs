
namespace Funds.Withdraw.ATM;

public interface IFetchFundsFromAtm
{
    Task ProcessAsync(FetchFundsFromAtmRequest request, CancellationToken cancellationToken = default);
}