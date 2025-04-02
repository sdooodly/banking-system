using System;

namespace BasicBankingSystem
{
    public class CheckingAccount : Account
    {
        public decimal OverdraftLimit { get; private set; }

        public CheckingAccount(string accountNumber, decimal initialBalance, decimal overdraftLimit)
            : base(accountNumber, initialBalance)
        {
            OverdraftLimit = overdraftLimit >= 0 ? overdraftLimit : 0;
        }

        public override void Withdraw(decimal amount)
        {
            if (amount > 0 && (Balance + OverdraftLimit) >= amount)
            {
                Balance -= amount;
                Console.WriteLine($"Withdrawal of {amount:C} successful. New balance: {Balance:C}");
            }
            else if (amount <= 0)
            {
                Console.WriteLine("Withdrawal amount must be positive.");
            }
            else
            {
                Console.WriteLine("Withdrawal exceeds available balance and overdraft limit.");
            }
        }

        public override void DisplayAccountInfo()
        {
            base.DisplayAccountInfo();
            Console.WriteLine($"Overdraft Limit: {OverdraftLimit:C}");
        }
    }
}