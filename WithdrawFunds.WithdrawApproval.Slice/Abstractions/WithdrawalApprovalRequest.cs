﻿using Funds.Abstractions;

namespace Funds.Withdraw.WithdrawFunds;

/// <summary>
/// Withdrawal approval request
/// </summary>
/// <param name="AccountId">Account identifier</param>
/// <param name="Data">Common transaction data</param>
/// <param name="InitiateMethod">The method of initiating the funds operation (like ATM, Teller, PayPal, etc.)</param>
/// <param name="Balance">The effective account balance (actual balance - the approved in progress withdrawal)</param>
public record WithdrawalApprovalRequest(AccountId AccountId,
                                        FundsTransactionData Data,
                                        FundsInitiateMethod InitiateMethod,
                                        double Balance);