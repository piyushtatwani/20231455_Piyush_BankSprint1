using System;

namespace BankSprint1
{
    public abstract class Account
    {
        // Protected fields accessible by child classes
        protected double balance;
        protected string accountName;
        protected string lastTransactionStatus;
        
        // Private field for encapsulation, accessible via public property
        private Customer accountOwner;

        // Public properties for external access
        public double Balance { get { return balance; } }
        public string AccountName { get { return accountName; } }
        public string LastTransactionStatus { get { return lastTransactionStatus; } }
        public Customer AccountOwner { get { return accountOwner; } }

        public Account(Customer owner, string accName, double initialBalance)
        {
            accountOwner = owner;
            accountName = accName;
            balance = initialBalance;
            lastTransactionStatus = "Account Created";
        }

        public virtual void Deposit(double amount)
        {
            if (amount > 0)
            {
                balance += amount;
                lastTransactionStatus = "Deposit Successful";
            }
            else
            {
                lastTransactionStatus = "Invalid Deposit Amount";
            }
        }

        // Abstract method forces child classes to implement specific logic
        public abstract void Withdraw(double amount);
        
        public virtual void CalculateInterest() 
        {
            // By default, no interest calculation
        }

        public string GetAccountInfo()
        {
            return $"Account Name: {accountName}, Balance: ${balance:F2}, Last Status: {lastTransactionStatus}";
        }
    }
}
