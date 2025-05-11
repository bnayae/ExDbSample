using Core.Abstractions;
using Funds.Abstractions;

namespace Funds.Withdraw.WithdrawFunds;

internal class WithdrawalApproval : ICommandHandler<WithdrawalApprovalRequest>
{
    private readonly ILogger<WithdrawalApproval> _logger;
    private readonly IEvDbWithdrawFundsRequestFactory _evDbFactory;

    public WithdrawalApproval(ILogger<WithdrawalApproval> logger,
                              IEvDbWithdrawFundsRequestFactory evDbFactory)
    {
        _logger = logger;
        _evDbFactory = evDbFactory;
    }

    async Task ICommandHandler<WithdrawalApprovalRequest>.ProcessAsync(
                                                WithdrawalApprovalRequest request,
                                                CancellationToken cancellationToken)
    {
        (AccountId accountId, FundsTransactionData data, FundsInitiateMethod initiateMethod, double balance) = request;

        IEvDbWithdrawFundsRequest stream = await _evDbFactory.GetAsync(accountId, cancellationToken);

        double sum = stream.Views.WithdrawalsInProcess
                                .Where(m => m.Data.Currency == request.Data.Currency)
                                .Sum(m => m.Data.Amount);
        if (balance - sum < data.Amount)
        {
            var e = new FundsWithdrawalApprovedEvent(accountId, data, initiateMethod);
            await stream.AddAsync(e);
            _logger.WithdrawalApproved(accountId);
        }
        else
        {
            var e = new FundsWithdrawalDeclinedEvent(accountId, data, initiateMethod);
            await stream.AddAsync(e);
            _logger.WithdrawalDenied(accountId);
        }

        await stream.StoreAsync(cancellationToken);
    }
}

