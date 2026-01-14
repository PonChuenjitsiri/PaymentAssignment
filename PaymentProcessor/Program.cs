using System.Text.Json;
using PaymentProcessor.Models;
using PaymentProcessor.Services;

class Program
{
    static void Main(string[] args)
    {
        string inputPath = "transactions.json"; 
        string outputPath = "report.json";      

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--input" && i + 1 < args.Length) inputPath = args[i + 1];
            if (args[i] == "--output" && i + 1 < args.Length) outputPath = args[i + 1];
        }

        Console.WriteLine($"Reading from: {inputPath}");
        
        if (!File.Exists(inputPath))
        {
            Console.WriteLine("Error Input file not found.");
            return;
        }

        string jsonContent = File.ReadAllText(inputPath);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        
        var allTransactions = JsonSerializer.Deserialize<List<TransactionInput>>(jsonContent, options) 
                              ?? new List<TransactionInput>();

        var service = new PaymentServices(); 

        var (validList, invalidList) = service.ValidTransactions(allTransactions);

        var duplicates = service.DetectDuplicates(validList);

        
        var successTransactions = validList
            .Where(t => t.Status?.ToUpper() == "SUCCESS")
            .DistinctBy(t => t.TransactionId)
            .ToList();
        decimal totalAmount = successTransactions.Sum(t => t.Amount);
        decimal avgAmount = successTransactions.Any() ? successTransactions.Average(t => t.Amount) : 0;
        decimal maxAmount = successTransactions.Any() ? successTransactions.Max(t => t.Amount) : 0;
        decimal minAmount = successTransactions.Any() ? successTransactions.Min(t => t.Amount) : 0;

        var statusCounts = validList
            .GroupBy(t => t.Status?.ToUpper() ?? "UNKNOWN")
            .ToDictionary(g => g.Key, g => g.Count());

        var report = new ReportOutput(
            TotalProcessed: allTransactions.Count,
            ValidCount: validList.Count,
            InvalidCount: invalidList.Count,
            InvalidRecords: invalidList,
            StatusCounts: statusCounts,
            Duplicates: duplicates,
            TotalAmount: totalAmount,
            AverageAmount: avgAmount,
            MaxAmount: maxAmount,
            MinAmount: minAmount
        );

        
        var outputJson = JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(outputPath, outputJson);

        Console.WriteLine($"Report saved to: {outputPath}");
        
        Console.WriteLine($"    - Valid: {validList.Count}, Invalid: {invalidList.Count}");
        Console.WriteLine($"    - Duplicates Groups: {duplicates.Count}");
    }
}