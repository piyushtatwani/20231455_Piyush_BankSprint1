using System;

namespace BankSprint1
{
    public class EverydayAccount : Account
    {
        // Constructor uses the base keyword to pass essential data
        public EverydayAccount(Customer owner, double initialBalance) 
            : base(owner, "Everyday Account", initialBalance)
        {
        }

        public override void Withdraw(double amount)
        {
            if (amount <= 0)
            {
                lastTransactionStatus = "Invalid Withdrawal Amount";
                return;
            }

            if (balance >= amount)
            {
                balance -= amount;
                lastTransactionStatus = "Withdrawal Successful";
            }
            else
            {
                // No overdraft allowed, no fees
                lastTransactionStatus = "Insufficient Funds. No Overdraft Allowed.";
            }
        }
    }
}
