namespace PaymentProcessor.Models;

public record InvalidRecord(
    string TransactionId,
    List<string> Reason
);
