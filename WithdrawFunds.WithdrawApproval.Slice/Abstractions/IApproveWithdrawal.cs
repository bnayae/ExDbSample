namespace Funds.Withdraw.WithdrawFunds;

public interface IApproveWithdrawal
{
    Task ProcessAsync(WithdrawalApprovalRequest request, CancellationToken cancellationToken = default);
}
