using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace BasicBankingSystem
{
    public class CheckingAccount : Account
    {
        public decimal OverdraftLimit { get; private set; }

        public CheckingAccount(string accountNumber, decimal initialBalance, decimal overdraftLimit, ILogger? logger = null)
            : base(accountNumber, initialBalance, logger)
        {
            OverdraftLimit = overdraftLimit >= 0 ? overdraftLimit : 0;
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

            if (amount > 0 && (Balance + OverdraftLimit) >= amount)
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
                Logger.LogWarning("Withdrawal exceeds available balance and overdraft limit.");
                activity?.AddEvent(new ActivityEvent("WithdrawFailedOverdraft"));
            }
        }

        public override void DisplayAccountInfo()
        {
            base.DisplayAccountInfo();
            Logger.LogInformation("Overdraft Limit: {Limit:C}", OverdraftLimit);
        }
    }
}