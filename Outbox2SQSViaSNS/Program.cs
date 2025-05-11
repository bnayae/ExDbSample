#pragma warning disable CA1303 // Do not pass literals as localized parameters

using Microsoft.Extensions;
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
    QueueName = "WithdrawApprover",
};

var reactToCalculateWithdrawalsCommission = new StreamSinkSetting
{
    DbName = DATABASE,
    CollectionName = "dev_Withdraw_Funds_Request_outbox",
    QueueName = "CalculateWithdrawalsCommission",
};

Console.WriteLine("Listening to Outbox");

await Task.WhenAll(
        reactToWithdrawalRequestedViaATM.ListenToOutbox(cancellationToken),
        reactToCalculateWithdrawalsCommission.ListenToOutbox(meta =>
                                                    // meta.Channel == "todo" && 
                                                    meta.MessageType == "calculate-withdrawals-commission",
                                                    cancellationToken));

// CalculateWithdrawalsCommissionMessage