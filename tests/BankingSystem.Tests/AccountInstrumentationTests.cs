using System;
using System.Diagnostics.Metrics;
using Xunit;
using BasicBankingSystem;

namespace BankingSystem.Tests
{
    public class AccountInstrumentationTests
    {
        [Fact]
        public void Deposit_Increments_Counters()
        {
            long depositCount = 0;
            double depositAmount = 0;

            using var listener = new MeterListener();
            listener.InstrumentPublished = (instrument, l) =>
            {
                if (instrument.Meter.Name == "BasicBankingSystem.Metrics")
                    l.EnableMeasurementEvents(instrument);
            };

            listener.SetMeasurementEventCallback<long>((instrument, measurement, tags, state) =>
            {
                if (instrument.Name == "account.deposit.count") depositCount += measurement;
            });

            listener.SetMeasurementEventCallback<double>((instrument, measurement, tags, state) =>
            {
                if (instrument.Name == "account.deposit.amount") depositAmount += measurement;
            });

            listener.Start();

            var acc = new SavingsAccount("TST1", 0m, 0.01m);
            acc.Deposit(100m);

            Assert.Equal(1, depositCount);
            Assert.Equal(100d, depositAmount, 3);
        }

        [Fact]
        public void Withdraw_Increments_Counters_For_Savings()
        {
            long withdrawCount = 0;
            double withdrawAmount = 0;

            using var listener = new MeterListener();
            listener.InstrumentPublished = (instrument, l) =>
            {
                if (instrument.Meter.Name == "BasicBankingSystem.Metrics")
                    l.EnableMeasurementEvents(instrument);
            };

            listener.SetMeasurementEventCallback<long>((instrument, measurement, tags, state) =>
            {
                if (instrument.Name == "account.withdraw.count") withdrawCount += measurement;
            });
            listener.SetMeasurementEventCallback<double>((instrument, measurement, tags, state) =>
            {
                if (instrument.Name == "account.withdraw.amount") withdrawAmount += measurement;
            });

            listener.Start();

            var acc = new SavingsAccount("TST2", 200m, 0.01m);
            acc.Withdraw(50m);

            Assert.Equal(1, withdrawCount);
            Assert.Equal(50d, withdrawAmount, 3);
        }

        [Fact]
        public void Withdraw_Increments_Counters_For_Checking_With_Overdraft()
        {
            long withdrawCount = 0;
            double withdrawAmount = 0;

            using var listener = new MeterListener();
            listener.InstrumentPublished = (instrument, l) =>
            {
                if (instrument.Meter.Name == "BasicBankingSystem.Metrics")
                    l.EnableMeasurementEvents(instrument);
            };

            listener.SetMeasurementEventCallback<long>((instrument, measurement, tags, state) =>
            {
                if (instrument.Name == "account.withdraw.count") withdrawCount += measurement;
            });
            listener.SetMeasurementEventCallback<double>((instrument, measurement, tags, state) =>
            {
                if (instrument.Name == "account.withdraw.amount") withdrawAmount += measurement;
            });

            listener.Start();

            var acc = new CheckingAccount("TST3", 10m, 100m);
            acc.Withdraw(50m);

            Assert.Equal(1, withdrawCount);
            Assert.Equal(50d, withdrawAmount, 3);
        }

        [Fact]
        public void DisplayAccountInfo_Increments_Display_Count()
        {
            long displayCount = 0;

            using var listener = new MeterListener();
            listener.InstrumentPublished = (instrument, l) =>
            {
                if (instrument.Meter.Name == "BasicBankingSystem.Metrics")
                    l.EnableMeasurementEvents(instrument);
            };

            listener.SetMeasurementEventCallback<long>((instrument, measurement, tags, state) =>
            {
                if (instrument.Name == "account.display.count") displayCount += measurement;
            });

            listener.Start();

            var acc = new SavingsAccount("TST4", 77m, 0.01m);
            acc.DisplayAccountInfo();

            Assert.Equal(1, displayCount);
        }
    }
}
