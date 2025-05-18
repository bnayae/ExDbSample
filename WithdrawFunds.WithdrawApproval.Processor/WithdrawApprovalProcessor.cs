// Ignore Spelling: evdb

using Core.Abstractions;
using Funds.Withdraw.RequestWithdrawFundsViaATM;
using Funds.Withdraw.WithdrawFunds;
using Microsoft.Extensions.Logging;

namespace WithdrawFunds.WithdrawApproval.Processor;

internal sealed class WithdrawApprovalProcessor : IProcessor<FundsWithdrawalRequestedViaAtmMessage, WithdrawalApprovalRequest>
{
    private readonly IEvDbAccountFundsFactory _evdbFactory;

    public WithdrawApprovalProcessor(
                    ILogger<WithdrawApprovalProcessor> logger,
                    IEvDbAccountFundsFactory evdbFactory)
    {
        _evdbFactory = evdbFactory;
    }

    async Task<WithdrawalApprovalRequest> IProcessor<FundsWithdrawalRequestedViaAtmMessage, WithdrawalApprovalRequest>
                    .ProcessAsync(FundsWithdrawalRequestedViaAtmMessage message,
                                 CancellationToken cancellationToken)
    {
        var stream = await _evdbFactory.GetAsync(message.AccountId, cancellationToken);
        var currency = message.Data.Currency;
        var balance = stream.Views.AccountBalance;
        var balanceOfCurrency = balance.GetValueOrDefault(currency);
        var withdrawalApprovalRequest = new WithdrawalApprovalRequest(message.AccountId,
                                                                      message.Data,
                                                                      message.InitiateMethod,
                                                                      balanceOfCurrency.Funds);
        return withdrawalApprovalRequest;
    }
}
