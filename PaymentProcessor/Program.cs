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

        var fileService = new FileService();  
        var paymentService = new PaymentServices();

        var transactions = fileService.ReadTransactions(inputPath);

        if (transactions.Count == 0) return;


        var (validList, invalidList) = paymentService.ValidTransactions(transactions);
        var duplicates = paymentService.DetectDuplicates(validList);
        var stats = paymentService.CalculateStats(validList);

        var statusCounts = paymentService.StatusCount(validList);

        var report = new ReportOutput(
            TotalProcessed: transactions.Count,
            ValidCount: validList.Count,
            InvalidCount: invalidList.Count,
            InvalidRecords: invalidList,
            StatusCounts: statusCounts,
            Duplicates: duplicates,
            TotalAmount: stats.TotalAmount,
            AverageAmount: stats.AverageAmount,
            MaxAmount: stats.MaxAmount,
            MinAmount: stats.MinAmount
        );


        fileService.SaveReport(outputPath, report);

        Console.WriteLine($"Report saved to: {outputPath}");

        Console.WriteLine($"    - Valid: {validList.Count}, Invalid: {invalidList.Count}");
        Console.WriteLine($"    - Duplicates Groups: {duplicates.Count}");
    }
}