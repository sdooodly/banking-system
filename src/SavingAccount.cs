using System;

namespace BasicBankingSystem
{
    public class SavingsAccount : Account
    {
        public decimal InterestRate { get; private set; }

        public SavingsAccount(string accountNumber, decimal initialBalance, decimal interestRate)
            : base(accountNumber, initialBalance) // Call the base class constructor
        {
            InterestRate = interestRate >= 0 ? interestRate : 0;
        }

        public void ApplyInterest()
        {
            decimal interest = Balance * InterestRate;
            Balance += interest;
            Console.WriteLine($"Interest of {interest:C} applied. New balance: {Balance:C}");
        }

        public override void Withdraw(decimal amount)
        {
            if (amount > 0 && Balance >= amount)
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
                Console.WriteLine("Insufficient funds for withdrawal.");
            }
        }

        public override void DisplayAccountInfo()
        {
            base.DisplayAccountInfo(); // Call the base class implementation
            Console.WriteLine($"Interest Rate: {InterestRate:P}"); // Display interest rate as percentage
        }
    }
}