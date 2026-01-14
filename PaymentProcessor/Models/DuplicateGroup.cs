namespace PaymentProcessor.Models;

public record DuplicateGroup(
    string Rule,
    List<string> TransactionIds
);