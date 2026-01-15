using PaymentProcessor.Models;

namespace PaymentProcessor.Services;

public class PaymentServices
{
    static readonly HashSet<string> validStatuses = new HashSet<string> { "SUCCESS", "FAILED", "PENDING" };

    public (List<TransactionInput>, List<InvalidRecord>) ValidTransactions(List<TransactionInput> transactions)
    {
        if (transactions == null) 
            return (new List<TransactionInput>(), new List<InvalidRecord>());
        var validTransactions = new List<TransactionInput>();
        var invalidRecords = new List<InvalidRecord>();

        foreach (var txn in transactions)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(txn.TransactionId))
                errors.Add("TransactionId is missing");

            if (string.IsNullOrWhiteSpace(txn.MerchantRef))
                errors.Add("MerchantRef is missing");

            if (txn.Amount <= 0)
                errors.Add($"Amount must be Greater 0 (Input: {txn.Amount})");

            if (string.IsNullOrWhiteSpace(txn.Currency) || txn.Currency.Length != 3)
                errors.Add($"Currency must be 3 letters (Input: {txn.Currency})");

            if (string.IsNullOrWhiteSpace(txn.Status) || !validStatuses.Contains(txn.Status.ToUpper()))
                errors.Add($"Invalid Status (Input: {txn.Status})");

            if (txn.CreatedAtUtc == default)
                errors.Add("Date is invalid");

            if (errors.Count > 0)
            {
                invalidRecords.Add(new InvalidRecord(txn.TransactionId, errors));
            }
            else
            {
                validTransactions.Add(txn);
            }
        }
        return (validTransactions, invalidRecords);
    }

    public List<DuplicateGroup> DetectDuplicates(List<TransactionInput> validTransactions)
    {
        if (validTransactions == null || !validTransactions.Any()) 
            return new List<DuplicateGroup>();
        var results = new List<DuplicateGroup>();

        var sameIdGroups = validTransactions
            .GroupBy(x => x.TransactionId)
            .Where(g => g.Count() > 1) 
            .ToList();

        foreach (var group in sameIdGroups)
        {
            results.Add(new DuplicateGroup(
                Rule: "Rule 1: Duplicate Transaction ID",
                TransactionIds: group.Select(t => t.TransactionId).ToList()
            ));
        }
        var sameDetailsGroups = validTransactions
            .GroupBy(x => new
            {
                x.MerchantRef,
                x.Amount,
                x.Currency,
                Date = x.CreatedAtUtc.Date
            })
            .Where(g => g.Count() > 1)
            .ToList();

        foreach (var group in sameDetailsGroups)
        {
            results.Add(new DuplicateGroup(
                Rule: "Rule 2: Repeated Transaction (Same Merchant/Amount/Day)",
                TransactionIds: group.Select(t => t.TransactionId).ToList()
            ));
        }

        return results;
    }

    public PaymentStats CalculateStats(List<TransactionInput> validTransactions)
    {
        if (validTransactions == null || !validTransactions.Any()) 
            return new PaymentStats(0, 0, 0, 0); 

        var successUnique = validTransactions
            .Where(t => t.Status?.ToUpper() == "SUCCESS")
            .DistinctBy(t => t.TransactionId) 
            .ToList();

        if (!successUnique.Any()) 
            return new PaymentStats(0, 0, 0, 0); 

        return new PaymentStats(
            TotalAmount: successUnique.Sum(t => t.Amount),
            AverageAmount: successUnique.Average(t => t.Amount),
            MaxAmount: successUnique.Max(t => t.Amount),
            MinAmount: successUnique.Min(t => t.Amount)
        );
    }

    public Dictionary<string, int> StatusCount(List<TransactionInput> validTransactions)
    {
        if (validTransactions == null || !validTransactions.Any()) 
            return new Dictionary<string, int>();
        return validTransactions
            .GroupBy(t => t.Status?.ToUpper() ?? "")
            .ToDictionary(g => g.Key, g => g.Count());

    }
}