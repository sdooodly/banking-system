using System;

namespace BasicBankingSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Creating account objects
            SavingsAccount savings = new SavingsAccount("SA123", 1000, 0.02m); // 2% interest
            CheckingAccount checking = new CheckingAccount("CA456", 500, 200); // Overdraft of $200

            // Performing operations
            savings.DisplayAccountInfo();
            savings.Deposit(10500);
            savings.Withdraw(200);
            savings.ApplyInterest();
            savings.DisplayAccountInfo();
            Console.WriteLine();

            checking.DisplayAccountInfo();
            checking.Deposit(22200);
            checking.Withdraw(750); // Should use overdraft
            checking.DisplayAccountInfo();
            checking.Withdraw(1);   // Should exceed limit
            Console.WriteLine();

            // Polymorphism example: Treating different account types through a common base class
            Account[] accounts = { savings, checking };
            foreach (Account account in accounts)
            {
                Console.WriteLine("--- Account Information (Polymorphic) ---");
                account.DisplayAccountInfo();
                Console.WriteLine();
            }
        }
    }
}