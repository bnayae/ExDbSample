// Ignore Spelling: Swimlanes

namespace Funds.Abstractions;
#pragma warning disable CA1034 // Nested types should not be visible

public static class FundsConstants
{
    public static class Swimlanes
    {
        public const string MONGODbConnectionKey = "EvDbMongoDBConnection";
        public const string DatabaseName = "funds-withdraw";
    }

    /// <summary>
    /// The channels for the funds withdrawal process.
    /// </summary>
    public static class Queues
    {
        public const string WithdrawApprover = "withdraw-approver";
        public const string CalculateWithdrawalsCommission = "calculate-withdrawals-commission"; // similar to CalculateWithdrawalsCommissionMessage.PAYLOAD_TYPE yet decided not to couple it
    }
    
}