namespace PaymentProcessor.Models;

public record TransactionInput(
    string TransactionId,
    string MerchantRef,
    decimal Amount,
    string Currency,
    string Status,
    DateTime CreatedAtUtc
);