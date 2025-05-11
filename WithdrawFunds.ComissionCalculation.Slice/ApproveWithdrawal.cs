namespace Funds.Withdraw.WithdrawFunds.WithdrawApproval.Slice;

internal class ApproveWithdrawal
{
    private readonly ILogger<ApproveWithdrawal> _logger;
    private readonly IFundsCommissionPolicy _commissionPolicy;
    private readonly IFundsSegmentation _segmentation;

    public ApproveWithdrawal(ILogger<ApproveWithdrawal> logger,
                          IFundsCommissionPolicy commissionPolicy,
                          IFundsSegmentation segmentation)
    {
        _logger = logger;
        _commissionPolicy = commissionPolicy;
        _segmentation = segmentation;
    }

    //public Task ApproveWithdrawalAsync(FundsWithdrawalRequested message)
    //{

    //}
}
