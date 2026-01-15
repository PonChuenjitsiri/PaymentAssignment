1. Build the project - dotnet build
2. Run the application - dotnet run --project PaymentProcessor or dotnet run --project PaymentProcessor -- --input transactions.json --output report.json
3. Run Tests - dotnet test

Assumptions:
1. Currency Handling - The system currently aggregates amounts regardless of currency Example( 100 USD + 100 THB = 200 )
2. Idempotency - When duplicate TransactionIds are found, the system processes the first valid 'SUCCESS' transaction encountered and ignores subsequent duplicates for the  summary.
3. transactionId doesn't have to be formatted TXID-xxx

Trade-offs:
1. Chosen in-memory processing for simplicity and speed in implementing complex grouping logic (LINQ) for duplicate detection.

AI tools:(Gemini)

-----Part A — Coding Task
1. Generate data in transactions.json for testing
2. I asked about do i need to put a File management to service because first i handle file in program.cs but in my perspective it a small project but in the end it suggest to put in service for clean code 
3. Fixing bug
4. Recheck a logic in Paymentservice i concern i miss anything 

-----Part B — Applied Scenario 
1. Research about payment gateway what i use is about Encode JWT