using Funds.Abstractions;
using Funds.Withdraw.WithdrawFunds;
using Microsoft.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
#pragma warning disable CA1848 // Use the LoggerMessage delegates

namespace Outbox2SQSViaSNS;

internal class Job : BackgroundService
{
    const string DATABASE = "funds_withdraw";
    private readonly ILogger<Job> _logger;

    #region StreamSinkSetting UREACT_TO_WITHDRAWAL_REQUESTED_VIA_ATM

    private readonly static StreamSinkSetting UREACT_TO_WITHDRAWAL_REQUESTED_VIA_ATM = new StreamSinkSetting
    {
        DbName = DATABASE,
        CollectionName = "dev_ATM_Funds_outbox",
        StreamName = "ReactToWithdrawalRequestedViaATM",
        QueueName = FundsConstants.Queues.WithdrawApprover,
    };

    #endregion //  StreamSinkSetting UREACT_TO_WITHDRAWAL_REQUESTED_VIA_ATM

    #region REACT_TO_CALCULATE_WITHDRAWALS_COMMISSION

    private readonly static StreamSinkSetting REACT_TO_CALCULATE_WITHDRAWALS_COMMISSION = new StreamSinkSetting
    {
        DbName = DATABASE,
        CollectionName = "dev_Withdraw_Funds_Request_outbox",
        QueueName = FundsConstants.Queues.CalculateWithdrawalsCommission,
    };

    #endregion //  REACT_TO_CALCULATE_WITHDRAWALS_COMMISSION

    public Job(ILogger<Job> logger)
    {
        _logger = logger;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogDebug("Listening to Outbox");

        TimeSpan sqsVisibilityTimeout = TimeSpan.FromMinutes(2);
        await Task.WhenAll(
                UREACT_TO_WITHDRAWAL_REQUESTED_VIA_ATM.ListenToOutbox(_logger, sqsVisibilityTimeout, stoppingToken),
                REACT_TO_CALCULATE_WITHDRAWALS_COMMISSION.ListenToOutbox(_logger, 
                                                            meta =>
                                                                            meta.Channel == WithdrawFundsChannels.Todo &&
                                                                            meta.MessageType == CalculateWithdrawalsCommissionMessage.PAYLOAD_TYPE,
                                                            sqsVisibilityTimeout,
                                                            stoppingToken));
    }
}
