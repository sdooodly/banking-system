using System;
using System.IO;
using System.Diagnostics;
using Serilog;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace BasicBankingSystem
{
    public class Program
    {
        private static readonly ActivitySource ActivitySource = new ActivitySource("BasicBankingSystem");

        public static void Main(string[] args)
        {
            // create a logs folder in the current working directory
            var baseDir = Directory.GetCurrentDirectory();
            var logsDir = Path.Combine(baseDir, "logs");
            Directory.CreateDirectory(logsDir);
            var logFile = Path.Combine(logsDir, $"run-{DateTime.UtcNow:yyyyMMdd-HHmmss}.log");

            // configure Serilog to write to console and a per-run file
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(logFile, rollingInterval: RollingInterval.Infinite)
                .CreateLogger();

            // configure OpenTelemetry Tracing with Console + OTLP exporters
            using var tracerProvider = Sdk.CreateTracerProviderBuilder()
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("BasicBankingSystem"))
                .AddSource("BasicBankingSystem")
                .AddConsoleExporter()
                .AddOtlpExporter() // sends to localhost:4317 by default
                .Build();

            // configure OpenTelemetry Metrics (MeterProvider) to export to Console + OTLP
            using var meterProvider = Sdk.CreateMeterProviderBuilder()
                .AddMeter("BasicBankingSystem.Metrics")
                .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("BasicBankingSystem"))
                .AddConsoleExporter()
                .AddOtlpExporter() // uses OTEL_EXPORTER_OTLP_ENDPOINT/OTEL_EXPORTER_OTLP_TRACES/metrics env vars
                .Build();

            Log.Information("Application starting. Log file: {LogFile}", logFile);

            // integrate Serilog with Microsoft.Extensions.Logging and create loggers for classes
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddSerilog());

            using (var activity = ActivitySource.StartActivity("RunDemo"))
            {
                // Creating account objects
                var savingsLogger = loggerFactory.CreateLogger<SavingsAccount>();
                var checkingLogger = loggerFactory.CreateLogger<CheckingAccount>();

                SavingsAccount savings = new SavingsAccount("SA123", 1000, 0.02m, savingsLogger); // 2% interest
                CheckingAccount checking = new CheckingAccount("CA456", 500, 200, checkingLogger); // Overdraft of $200

                Log.Information("Created accounts");

                // Performing operations
                savings.DisplayAccountInfo();
                savings.Deposit(10500);
                savings.Withdraw(200);
                savings.ApplyInterest();
                savings.DisplayAccountInfo();

                checking.DisplayAccountInfo();
                checking.Deposit(22200);
                checking.Withdraw(750); // Should use overdraft
                checking.DisplayAccountInfo();
                checking.Withdraw(1);   // Should exceed limit

                // Polymorphism example: Treating different account types through a common base class
                Account[] accounts = { savings, checking };
                foreach (Account account in accounts)
                {
                        Log.Information("--- Account Information (Polymorphic) ---");
                        // pass a logger for the polymorphic display
                        account.DisplayAccountInfo();
                }
            }

            Log.Information("Application finished");
            Log.CloseAndFlush();
        }
    }
}