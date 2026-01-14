using System.Text.Json;
using PaymentProcessor.Models;

namespace PaymentProcessor.Services;

public class FileService
{
    public List<TransactionInput> ReadTransactions(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Error: {filePath} not found.");
            return new List<TransactionInput>();
        }

        try
        {
            string jsonContent = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            
            var result = JsonSerializer.Deserialize<List<TransactionInput>>(jsonContent, options);
            return result ?? new List<TransactionInput>();
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error reading JSON: {ex.Message}");
            return new List<TransactionInput>();
        }
    }

    public void SaveReport(string filePath, ReportOutput report)
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonOutput = JsonSerializer.Serialize(report, options);
            
            File.WriteAllText(filePath, jsonOutput);
            Console.WriteLine($"Report saved successfully to: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving file: {ex.Message}");
        }
    }
}