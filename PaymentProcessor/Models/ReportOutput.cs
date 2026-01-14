namespace PaymentProcessor.Models;

public record ReportOutput(
    int TotalProcessed,
    int ValidCount,
    int InvalidCount,
    Dictionary<string, int> StatusCounts, 
    List<DuplicateGroup> Duplicates,      
    decimal TotalAmount,
    decimal AverageAmount,
    decimal MaxAmount,
    decimal MinAmount
);