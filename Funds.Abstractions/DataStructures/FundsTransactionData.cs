using System.Collections.Immutable;
using System.ComponentModel;

namespace Funds.Abstractions;

/// <summary>
/// 
/// </summary>
/// <param name="PaymentId">Id of financial operation like moving money from one account to another (can related to multiple transactons)</param>
/// <param name="TransactionId">Id of a financial execution unit like withdrawal or deposit</param>
/// <param name="TransactionDate">Date of a financial execution unit like withdrawal or deposit</param>
/// <param name="Currency">The currency of the transaction</param>
/// <param name="FlowContexts">List of flows context (flow can be nested like having transfer..withdrawal..deposit)</param>
/// <param name="Amount">The money amount</param>
public readonly partial record struct FundsTransactionData(
                                                Guid PaymentId,
                                                Guid TransactionId,
                                                DateTimeOffset TransactionDate,
                                                Currency Currency,
                                                ImmutableArray<FundsFlowContext> FlowContexts,
                                                double Amount);
