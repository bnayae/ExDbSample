#pragma warning disable CA1303 // Do not pass literals as localized parameters

using static Microsoft.Extensions.Extensions;
const string DATABASE = "funds.withdraw";

using var cts = new CancellationTokenSource();
var cancellationToken = cts.Token;

var reactToWithdrawalRequestedViaATM = new StreamSinkSetting
{
    DbName = DATABASE,
    CollectionName = "ReactToWithdrawalRequestedViaATM", // TODO: check the full name
    StreamName = "ReactToWithdrawalRequestedViaATM",
    QueueName = "WithdrawApprover",
};

Console.WriteLine("Listening to Outbox");

await Task.WhenAll( 
        reactToWithdrawalRequestedViaATM.ListenToOubox(cancellationToken),
        Task.CompletedTask);