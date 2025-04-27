namespace EvDbSample.Abstractions;

public readonly partial record struct FundsDepositedData(
                                                            Guid transactionId,
                                                            DateTimeOffset transactionTime,
                                                            double Amount,
                                                            Currency Currency);
