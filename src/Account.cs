using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace BasicBankingSystem
{
    public abstract class Account
    {
        // OpenTelemetry Meter and counters used for metrics export. Tests should capture these via a MeterListener.
        private static readonly System.Diagnostics.Metrics.Meter MetricMeter = new System.Diagnostics.Metrics.Meter("BasicBankingSystem.Metrics", "1.0.0");
        private static readonly System.Diagnostics.Metrics.Counter<long> DepositCountCounter = MetricMeter.CreateCounter<long>("account.deposit.count");
        private static readonly System.Diagnostics.Metrics.Counter<double> DepositAmountCounter = MetricMeter.CreateCounter<double>("account.deposit.amount");
        private static readonly System.Diagnostics.Metrics.Counter<long> WithdrawCountCounter = MetricMeter.CreateCounter<long>("account.withdraw.count");
        private static readonly System.Diagnostics.Metrics.Counter<double> WithdrawAmountCounter = MetricMeter.CreateCounter<double>("account.withdraw.amount");
        private static readonly System.Diagnostics.Metrics.Counter<long> DisplayCountCounter = MetricMeter.CreateCounter<long>("account.display.count");
        private static readonly ActivitySource ActivitySource = new ActivitySource("BasicBankingSystem.Accounts");
        public string AccountNumber { get; private set; }
        public decimal Balance { get; protected set; }
        protected ILogger Logger { get; }

        public Account(string accountNumber, decimal initialBalance, ILogger? logger = null)
        {
            AccountNumber = accountNumber;
            Balance = initialBalance >= 0 ? initialBalance : 0;
            Logger = logger ?? NullLogger.Instance;
        }

        public virtual void Deposit(decimal amount)
        {
            using var activity = ActivitySource.StartActivity("Deposit", ActivityKind.Internal);
            using var scope = Logger.BeginScope("Account:{AccountNumber}", AccountNumber);

            if (activity is not null)
            {
                activity.SetTag("account.number", AccountNumber);
                activity.SetTag("operation", "deposit");
                activity.SetTag("amount", amount);
            }

            if (amount > 0)
            {
                Balance += amount;
                Logger.LogInformation("Deposit of {Amount:C} successful. New balance: {Balance:C}", amount, Balance);
                activity?.AddEvent(new ActivityEvent("DepositSucceeded"));

                // update OpenTelemetry counters (MeterListener or exporters will observe these)
                DepositCountCounter.Add(1, new KeyValuePair<string, object?>("account.number", AccountNumber));
                DepositAmountCounter.Add((double)amount, new KeyValuePair<string, object?>("account.number", AccountNumber));
            }
            else
            {
                Logger.LogWarning("Deposit amount must be positive.");
                activity?.AddEvent(new ActivityEvent("DepositFailed"));
            }
        }

        public abstract void Withdraw(decimal amount);

        public virtual void DisplayAccountInfo()
        {
            // instrumentation: record that account info was viewed via metric counter
            DisplayCountCounter.Add(1, new KeyValuePair<string, object?>("account.number", AccountNumber));
            Logger.LogInformation(new EventId(1000, "AccountInfo"), "Account Number: {AccountNumber}", AccountNumber);
            Logger.LogInformation(new EventId(1001, "AccountBalance"), "Current Balance: {Balance:C}", Balance);
        }

        // Allow derived classes to record withdraw metrics using a shared method
        protected static void RecordWithdraw(decimal amount, string accountNumber)
        {
            WithdrawCountCounter.Add(1, new KeyValuePair<string, object?>("account.number", accountNumber));
            WithdrawAmountCounter.Add((double)amount, new KeyValuePair<string, object?>("account.number", accountNumber));
        }
    }
}


