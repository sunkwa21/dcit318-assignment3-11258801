using System;
using System.Collections.Generic;

namespace FinanceManagementSystem
{
    // --- Custom exception for data integrity ---
    public class InvalidTransactionException : Exception
    {
        public InvalidTransactionException(string message) : base(message) { }
    }

    // --- Core model using a record (immutable) ---
    public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

    // --- Payment processing interface ---
    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    // --- Concrete processors ---
    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Validate(transaction);
            Console.WriteLine($"[BankTransfer] Processed bank transfer of {transaction.Amount:C} for '{transaction.Category}' (Id: {transaction.Id}).");
        }

        private void Validate(Transaction t)
        {
            if (t.Amount < 0) throw new InvalidTransactionException("Transaction amount cannot be negative.");
            if (string.IsNullOrWhiteSpace(t.Category)) throw new InvalidTransactionException("Transaction must have a category.");
        }
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Validate(transaction);
            Console.WriteLine($"[MobileMoney] Mobile money payment of {transaction.Amount:C} recorded for '{transaction.Category}' (Id: {transaction.Id}).");
        }

        private void Validate(Transaction t)
        {
            if (t.Amount < 0) throw new InvalidTransactionException("Transaction amount cannot be negative.");
            if (string.IsNullOrWhiteSpace(t.Category)) throw new InvalidTransactionException("Transaction must have a category.");
        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Validate(transaction);
            Console.WriteLine($"[CryptoWallet] Crypto wallet transfer: {transaction.Amount:C} for '{transaction.Category}' (Id: {transaction.Id}).");
        }

        private void Validate(Transaction t)
        {
            if (t.Amount < 0) throw new InvalidTransactionException("Transaction amount cannot be negative.");
            if (string.IsNullOrWhiteSpace(t.Category)) throw new InvalidTransactionException("Transaction must have a category.");
        }
    }

    // --- Base Account class ---
    public class Account
    {
        public string AccountNumber { get; }
        public decimal Balance { get; protected set; }

        public Account(string accountNumber, decimal initialBalance)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
                throw new ArgumentException("Account number cannot be empty.", nameof(accountNumber));
            if (initialBalance < 0)
                throw new ArgumentException("Initial balance cannot be negative.", nameof(initialBalance));

            AccountNumber = accountNumber;
            Balance = initialBalance;
        }

        // virtual method to allow specialization
        public virtual void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount < 0)
                throw new InvalidTransactionException("Transaction amount cannot be negative.");

            // Default behaviour: deduct
            Balance -= transaction.Amount;
            Console.WriteLine($"[Account] Applied transaction {transaction.Id}. New balance: {Balance:C}");
        }
    }

    // --- Sealed SavingsAccount ---
    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance)
            : base(accountNumber, initialBalance) { }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount < 0)
                throw new InvalidTransactionException("Transaction amount cannot be negative.");

            if (transaction.Amount > Balance)
            {
                Console.WriteLine("Insufficient funds");
                return;
            }

            Balance -= transaction.Amount;
            Console.WriteLine($"[SavingsAccount] Transaction {transaction.Id} of {transaction.Amount:C} applied. Updated balance: {Balance:C}");
        }
    }

    // --- Application that integrates everything ---
    public class FinanceApp
    {
        private readonly List<Transaction> _transactions = new();

        public void Run()
        {
            try
            {
                // i. Instantiate a SavingsAccount
                var savings = new SavingsAccount("SA-1001", 1000m);

                // ii. Create three Transaction records
                var t1 = new Transaction(1, DateTime.Now, 150.50m, "Groceries");
                var t2 = new Transaction(2, DateTime.Now, 300m, "Utilities");
                var t3 = new Transaction(3, DateTime.Now, 1200m, "Entertainment"); // intentionally larger than balance to show insufficient funds

                // iii. Process each transaction with the specified processors
                ITransactionProcessor p1 = new MobileMoneyProcessor();
                ITransactionProcessor p2 = new BankTransferProcessor();
                ITransactionProcessor p3 = new CryptoWalletProcessor();

                p1.Process(t1);
                p2.Process(t2);
                p3.Process(t3);

                // iv. Apply each transaction to the SavingsAccount
                savings.ApplyTransaction(t1);
                savings.ApplyTransaction(t2);
                savings.ApplyTransaction(t3);

                // v. Add all transactions to _transactions
                _transactions.Add(t1);
                _transactions.Add(t2);
                _transactions.Add(t3);

                // Optional: summary
                Console.WriteLine("\n--- Transactions Recorded ---");
                foreach (var tx in _transactions)
                {
                    Console.WriteLine($"Id:{tx.Id}, Date:{tx.Date}, Amount:{tx.Amount:C}, Category:{tx.Category}");
                }

                Console.WriteLine($"\nFinal balance for account {savings.AccountNumber}: {savings.Balance:C}");
            }
            catch (InvalidTransactionException ex)
            {
                Console.WriteLine($"Transaction validation error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }
    }

    // --- Program entry point ---
    class Program
    {
        static void Main(string[] args)
        {
            var app = new FinanceApp();
            app.Run();
        }
    }
}
