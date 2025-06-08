using Core.Abstractions;
using Funds.Abstractions;

namespace Funds.Withdraw.WithdrawFunds.WithdrawApproval.Slice;

internal class CalculateWithdrawCommissionCommand : ICommandHandler<CalculateWithdrawCommissionRequest>
{
    private readonly ILogger<CalculateWithdrawCommissionCommand> _logger;
    private readonly IEvDbWithdrawFundsRequestFactory _evDbFactory;

    public CalculateWithdrawCommissionCommand(
                              ILogger<CalculateWithdrawCommissionCommand> logger,
                              IEvDbWithdrawFundsRequestFactory evDbFactory)
    {
        _logger = logger;
        _evDbFactory = evDbFactory;
    }

    async Task ICommandHandler<CalculateWithdrawCommissionRequest>.ExecuteAsync(
                                            CalculateWithdrawCommissionRequest request,
                                            CancellationToken cancellationToken)
    {
        (AccountId accountId, FundsTransactionData data, FundsInitiateMethod initiateMethod, Commission commission) = request;

        IEvDbWithdrawFundsRequest stream = await _evDbFactory.GetAsync(accountId, cancellationToken);

        var e = new WithdrawCommissionCalculatedEvent(accountId, data, initiateMethod, commission);
        await stream.AppendAsync(e);
        _logger.WithdrawCommissionCalculated(accountId);
    }
}
