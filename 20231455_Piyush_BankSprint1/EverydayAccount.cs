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
                lastTransactionStatus = "Invalid Withdrawal Amount: Amount must be greater than zero.";
                throw new BankingException(lastTransactionStatus, accountName, BankingErrorType.InvalidAmount);
            }

            if (balance >= amount)
            {
                balance -= amount;
                lastTransactionStatus = "Withdrawal Successful";
            }
            else
            {
                // No overdraft allowed, no fees
                lastTransactionStatus = $"[Everyday Account] Insufficient funds. Attempted withdrawal of ${amount:F2} exceeds available balance of ${balance:F2}. Everyday accounts do not permit overdraft.";
                throw new InsufficientFundsException(lastTransactionStatus, accountName, amount, balance);
            }
        }
    }
}
