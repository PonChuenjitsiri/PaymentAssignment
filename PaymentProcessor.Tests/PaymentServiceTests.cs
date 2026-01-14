using Xunit;
using PaymentProcessor.Models;
using PaymentProcessor.Services;
using System.Collections.Generic;
using System;
using System.Linq;

namespace PaymentProcessor.Tests
{
    public class PaymentServiceTests
    {
        [Fact]
        public void ValidTransactionsTest()
        {
            var service = new PaymentServices();
            var inputs = new List<TransactionInput>
            {
                new TransactionInput("T1", "M1", 100, "THB", "SUCCESS", DateTime.Now),
                
                new TransactionInput("T2", "M1", -50, "THB", "SUCCESS", DateTime.Now), 

                new TransactionInput("T2", "M1", 0, "THB", "SUCCESS", DateTime.Now), 
                
                new TransactionInput("T3", "M1", 100, "XY", "SUCCESS", DateTime.Now),

                new TransactionInput("T4", "M1", 100, "THB", "SUCCESS", default) ,

                new TransactionInput("T222", "M1", 100, "THB", "asdasda", default) ,

                new TransactionInput("", "", 0, "", "", default) 
            };

            var (valid, invalid) = service.ValidTransactions(inputs);

            Assert.Single(valid);          
            Assert.Equal(6, invalid.Count); 

        }

        [Fact]
        public void DetectDuplicateTest()
        {
            var service = new PaymentServices();
            var inputs = new List<TransactionInput>
            {
                new TransactionInput("T-DUP", "M1", 100, "THB", "SUCCESS", DateTime.Parse("2023-01-01")),
                new TransactionInput("T-DUP", "M2", 200, "USD", "FAILED",  DateTime.Parse("2023-01-02")),

                new TransactionInput("T-A", "M3", 500, "THB", "SUCCESS", DateTime.Parse("2023-05-05T10:00:00")),
                new TransactionInput("T-B", "M3", 500, "THB", "SUCCESS", DateTime.Parse("2023-05-05T14:00:00")), 
            };

            var duplicates = service.DetectDuplicates(inputs);

            Assert.Equal(2, duplicates.Count);
            
            var rule1 = duplicates.FirstOrDefault(d => d.Rule.Contains("Rule 1"));
            Assert.NotNull(rule1);
            Assert.Equal(2, rule1.TransactionIds.Count); 

            var rule2 = duplicates.FirstOrDefault(d => d.Rule.Contains("Rule 2"));
            Assert.NotNull(rule2);
            Assert.Equal(2, rule2.TransactionIds.Count);
        }

        [Fact]
        public void CalculateStatsTest()
        {
            var service = new PaymentServices();
            var inputs = new List<TransactionInput>
            {
                new TransactionInput("TX-1", "M1", 100, "THB", "SUCCESS", DateTime.Now),
                
                new TransactionInput("TX-1", "M1", 200, "THB", "SUCCESS", DateTime.Now),

                new TransactionInput("TX-2", "M1", 50, "THB", "SUCCESS", DateTime.Now),
                new TransactionInput("TX-2", "M1", 50, "THB", "SUCCESS", DateTime.Now),

                new TransactionInput("TX-3", "M1", 5000, "THB", "FAILED", DateTime.Now)
            };

            var stats = service.CalculateStats(inputs);

            
            Assert.Equal(150, stats.TotalAmount);
            Assert.Equal(75, stats.AverageAmount); 
            Assert.Equal(100, stats.MaxAmount);
            Assert.Equal(50, stats.MinAmount);
        }
    }
}