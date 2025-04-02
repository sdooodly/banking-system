using System;

namespace BasicBankingSystem
{
    public abstract class Account
    {
        // declares a public property that holds a string balue and can be read from anywhere but can be set only from Account class, 
        // typically in the constructor where Account object is created 
        public string AccountNumber {get; private set;}
        // accessible within this class and derived classes
        public decimal Balance {get; protected set;}
        //constructor for Account class. it sets up the inital state of new account
        public Account(string accountNumber, decimal initialBalance)
        {
            AccountNumber = accountNumber;
            Balance = initialBalance >= 0 ? initialBalance : 0; // ensure inital balnce is not negative
        }
        public virtual void Deposit(decimal amount)
        {
            if (amount > 0)
            {
                Balance += amount;
                Console.WriteLine($"Deposit of {amount:C} successful. New balance: {Balance:C}");
            }
            else
            {
                Console.WriteLine("Deposit amount must be positive.");
            }
        }
        public abstract void Withdraw(decimal amount); // Abstract method, to be implemented by derived classes

        public virtual void DisplayAccountInfo()
        {
            Console.WriteLine($"Account Number: {AccountNumber}");
            Console.WriteLine($"Current Balance: {Balance:C}");
        }
    }
}


