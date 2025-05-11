using Core.Abstractions;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;

namespace Funds.Withdraw.WithdrawFunds;

internal sealed class CommissionCalculationProcessor : IProcessorToCommandBridge<CalculateWithdrawalsCommissionMessage, CalculateWithdrawCommissionRequest>
{
    private readonly IFundsCommissionPolicy _commissionPolicy;
    private readonly IFundsSegmentation _segmentation;

    public CommissionCalculationProcessor(IFundsCommissionPolicy commissionPolicy,
                                          IFundsSegmentation segmentation)
    {
        _commissionPolicy = commissionPolicy;
        _segmentation = segmentation;
    }

    async Task<CalculateWithdrawCommissionRequest> IProcessorToCommandBridge<CalculateWithdrawalsCommissionMessage, CalculateWithdrawCommissionRequest>
                                .BridgeAsync(CalculateWithdrawalsCommissionMessage message, CancellationToken cancellationToken)
    {
        ImmutableArray<Segment> seggments = await _segmentation.GetSegmentsAsync(message.AccountId, cancellationToken);
        Commission commission = await _commissionPolicy.GetCommissionAsync(seggments, cancellationToken);
        var request = new CalculateWithdrawCommissionRequest(message.AccountId,
                                                           message.Data,
                                                           message.InitiateMethod,
                                                           commission);
        return request;
    }

   
}
