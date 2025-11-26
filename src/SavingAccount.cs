using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace BasicBankingSystem
{
    public class SavingsAccount : Account
    {
        public decimal InterestRate { get; private set; }

        public SavingsAccount(string accountNumber, decimal initialBalance, decimal interestRate, ILogger? logger = null)
            : base(accountNumber, initialBalance, logger)
        {
            InterestRate = interestRate >= 0 ? interestRate : 0;
        }

        public void ApplyInterest()
        {
            decimal interest = Balance * InterestRate;
            Balance += interest;
            Logger.LogInformation("Interest of {Interest:C} applied. New balance: {Balance:C}", interest, Balance);
        }

        public override void Withdraw(decimal amount)
        {
            using var activity = new ActivitySource("BasicBankingSystem.Accounts").StartActivity("Withdraw", ActivityKind.Internal);
            using var scope = Logger.BeginScope("Account:{AccountNumber}", AccountNumber);

            if (activity is not null)
            {
                activity.SetTag("account.number", AccountNumber);
                activity.SetTag("operation", "withdraw");
                activity.SetTag("amount", amount);
            }

            if (amount > 0 && Balance >= amount)
            {
                Balance -= amount;
                Logger.LogInformation("Withdrawal of {Amount:C} successful. New balance: {Balance:C}", amount, Balance);
                activity?.AddEvent(new ActivityEvent("WithdrawSucceeded"));
                // record withdraw metrics via base helper
                RecordWithdraw(amount, AccountNumber);
            }
            else if (amount <= 0)
            {
                Logger.LogWarning("Withdrawal amount must be positive.");
                activity?.AddEvent(new ActivityEvent("WithdrawFailed"));
            }
            else
            {
                Logger.LogWarning("Insufficient funds for withdrawal.");
                activity?.AddEvent(new ActivityEvent("WithdrawFailedInsufficientFunds"));
            }
        }

        public override void DisplayAccountInfo()
        {
            base.DisplayAccountInfo();
            Logger.LogInformation("Interest Rate: {Rate:P}", InterestRate);
        }
    }
}