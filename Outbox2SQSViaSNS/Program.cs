#pragma warning disable CA1303 // Do not pass literals as localized parameters

using Funds.Abstractions;
using Microsoft.Extensions;
using Funds.Withdraw.WithdrawFunds;

using static Microsoft.Extensions.Extensions;

const string DATABASE = "funds_withdraw";

using var cts = new CancellationTokenSource();
var cancellationToken = cts.Token;

// TODO: environment aware

var reactToWithdrawalRequestedViaATM = new StreamSinkSetting
{
    DbName = DATABASE,
    CollectionName = "dev_ATM_Funds_outbox",
    StreamName = "ReactToWithdrawalRequestedViaATM",
    QueueName = FundsConstants.Queues.WithdrawApprover,
};

var reactToCalculateWithdrawalsCommission = new StreamSinkSetting
{
    DbName = DATABASE,
    CollectionName = "dev_Withdraw_Funds_Request_outbox",
    QueueName = FundsConstants.Queues.CalculateWithdrawalsCommission,
};

Console.WriteLine("Listening to Outbox");

await Task.WhenAll(
        reactToWithdrawalRequestedViaATM.ListenToOutbox(cancellationToken),
        reactToCalculateWithdrawalsCommission.ListenToOutbox(meta =>
                                                    meta.Channel == WithdrawFundsChannels.Todo && 
                                                    meta.MessageType == CalculateWithdrawalsCommissionMessage.PAYLOAD_TYPE,
                                                    cancellationToken));

// CalculateWithdrawalsCommissionMessage