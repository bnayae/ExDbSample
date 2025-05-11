// Ignore Spelling: evdb

using Core.Abstractions;
using Funds.Withdraw.RequestWithdrawFundsViaATM;
using Funds.Withdraw.WithdrawFunds;
using Microsoft.Extensions.Logging;

namespace WithdrawFunds.WithdrawApproval.Processor;

public sealed class WithdrawApprovalBridge : IProcessorToCommandBridge<FundsWithdrawalRequestedViaAtmMessage, WithdrawalApprovalRequest>
{
    private readonly IEvDbAccountFundsFactory _evdbFactory;

    public WithdrawApprovalBridge(ILogger<WithdrawApprovalBridge> logger, IEvDbAccountFundsFactory evdbFactory)
    {
        _evdbFactory = evdbFactory;
    }

    async Task<WithdrawalApprovalRequest> IProcessorToCommandBridge<FundsWithdrawalRequestedViaAtmMessage, WithdrawalApprovalRequest>
                    .BridgeAsync(FundsWithdrawalRequestedViaAtmMessage request,
                                 CancellationToken cancellationToken)
    {
        var stream = await _evdbFactory.GetAsync(request.AccountId, cancellationToken);
        var currency = request.Data.Currency;
        var balance = stream.Views.AccountBalance;
        var balanceOfCurrency = balance.GetValueOrDefault(currency);
        var withdrawalApprovalRequest = new WithdrawalApprovalRequest(request.AccountId,
                                                                      request.Data,
                                                                      request.InitiateMethod,
                                                                      balanceOfCurrency.Funds);
        return withdrawalApprovalRequest;
    }
}
