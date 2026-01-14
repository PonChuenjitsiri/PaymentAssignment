namespace PaymentProcessor.Models;

public record PaymentStats(
    decimal TotalAmount,
    decimal AverageAmount,
    decimal MaxAmount,
    decimal MinAmount
);