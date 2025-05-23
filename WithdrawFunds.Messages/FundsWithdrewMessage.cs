﻿using Funds.Abstractions;

namespace Funds.Withdraw.WithdrawFunds;

/// <summary>
/// Withdrawals commission calculation
/// </summary>
/// <param name="AccountId">Account identifier</param>
/// <param name="Data">Common transaction data</param>
/// <param name="InitiateMethod">The method of initiating the funds operation (like ATM, Teller, PayPal, etc.)</param>
[EvDbDefineMessagePayload("funds-withdrew")]
public readonly partial record struct FundsWithdrewMessage(AccountId AccountId,
                                                      FundsTransactionData Data,
                                                      FundsInitiateMethod InitiateMethod);