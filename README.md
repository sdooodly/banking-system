# Basic Banking System (C# OOP)

## Learning Objectives

* Classes are like cookie cutters. `Account`, `SavingsAccount`, `CheckingAccount` are our cutters.
    * The `Account` class, the `SavingsAccount` class, and the `CheckingAccount` class are all examples of classes.
* Objects are the cookies. When we say `new SavingsAccount()`, we're using the "SavingsAccount" cutter to make a specific savings account cookie!
    * The `savings` variable (e.g., `SavingsAccount savings = new SavingsAccount(...)`) holds an object of the `SavingsAccount` class. Similarly, `checking` holds an object of the `CheckingAccount` class.
* Encapsulation is the cookie jar. We keep the secret recipe (the data like balance) safe inside each cookie (object). You can only interact with it using specific actions (methods) we've allowed, like "deposit" or "withdraw."
    *  The `Balance` property in the `Account` class has a `protected set;`, meaning its modification is controlled within the `Account` class and its children through methods like `Deposit()` and `Withdraw()`. The `AccountNumber` has a `private set;`, restricting its modification entirely after creation.
* Inheritance is like a family recipe. `SavingsAccount` and `CheckingAccount` are like variations of the main `Account` cookie. They automatically get the basic recipe (account number, balance) and then add their own special ingredients (interest, overdraft).
    * The line `public class SavingsAccount : Account` demonstrates inheritance, where `SavingsAccount` inherits from the `Account` class.
* Abstraction is like a "Cookie" label. The `Account` cookie cutter is a bit abstract – you can't just make a generic "Cookie." You always make a *specific* type like "Savings" or "Checking." It focuses on the idea of an account without all the details.
    * The `Account` class is declared as `public abstract class Account`, making it an abstract class that cannot be directly instantiated. The `public abstract void Withdraw(decimal amount);` declares an abstract method that derived classes *must* implement.
* Polymorphism is like "Bake the cookie!". If you have a bunch of different cookies (Savings, Checking) and you say "Bake the cookie!", each one will bake in its own special way. In code, if you tell an account to "show info," a savings account will show its interest, and a checking account will show its overdraft – same instruction, different results!
    * The `Withdraw(decimal amount)` method is `virtual` in the `Account` class and `override` in `SavingsAccount` and `CheckingAccount` to handle withdrawals differently based on the account type. Similarly, `DisplayAccountInfo()` is virtual and overridden to show specific details for each account type.


## How to Run

1.  Install [.NET SDK](https://dotnet.microsoft.com/download).
2.  Navigate to project root, then `cd src`.
3.  Run: `dotnet run`.
4.  Observe console output.

## Structure

banking-system/     
├── src/    
│   ├── Account.cs         # Abstract base account class   
│   ├── SavingsAccount.cs  # Savings account with interest   
│   ├── CheckingAccount.cs # Checking account with overdraft   
│   └── Program.cs         # Main execution   
└── README.md  


## Next Steps

- transaction history
- user interface
- error handling
- data persistence
- more account types
- security

## Logs & OpenTelemetry

- **Log files**: Each run writes a per-run structured log file to the `logs/` folder in the repository root. Files are named like `run-YYYYMMDD-HHMMSS.log`.
- **Run and view logs**:
```bash
dotnet run --project src
ls -lah logs
tail -n 200 logs/run-*.log
```
- **OpenTelemetry (OTLP)**: The application uses OpenTelemetry tracing with Console and OTLP exporters. By default the OTLP exporter targets `localhost:4317`.
- **Change OTLP endpoint** (example using environment variables):
```bash
export OTEL_EXPORTER_OTLP_ENDPOINT=http://collector.example.com:4317
dotnet run --project src
```
- **Notes**: Console output is now produced via the logger (Serilog) and written to both console and the per-run log file. Traces are also printed to console by the OTel Console exporter and sent to the OTLP endpoint when configured.
