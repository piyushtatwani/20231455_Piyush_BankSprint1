using System;

namespace BankSprint1
{
    public class OmniAccount : Account
    {
        private double interestRate = 0.04;
        private double overdraftLimit = 500.0;
        private double failedTransactionFee = 10.0;
        private double interestThreshold = 1000.0;

        // Constructor uses the base keyword
        public OmniAccount(Customer owner, double initialBalance) 
            : base(owner, "Omni Account", initialBalance)
        {
        }

        public override void Withdraw(double amount)
        {
            if (amount <= 0)
            {
                lastTransactionStatus = "Invalid Withdrawal Amount: Amount must be greater than zero.";
                throw new BankingException(lastTransactionStatus, accountName, BankingErrorType.InvalidAmount);
            }

            if (balance + overdraftLimit >= amount)
            {
                balance -= amount;
                lastTransactionStatus = "Withdrawal Successful";
            }
            else
            {
                // Charge fee if withdrawal exceeds balance + overdraft
                double fee = AccountOwner.CalculateFee(failedTransactionFee);
                double originalBalance = balance;
                balance -= fee;
                lastTransactionStatus = $"[Omni Account] Overdraft limit exceeded. Attempted withdrawal of ${amount:F2} exceeds available balance (${originalBalance:F2}) plus ${overdraftLimit:F2} overdraft limit. A failed transaction fee of ${fee:F2} has been charged.";
                throw new OverdraftLimitExceededException(lastTransactionStatus, accountName, amount, originalBalance, overdraftLimit);
            }
        }

        public override void CalculateInterest()
        {
            if (balance > interestThreshold)
            {
                double interest = (balance - interestThreshold) * interestRate;
                balance += interest;
                lastTransactionStatus = $"Interest Applied: ${interest:F2}";
            }
            else
            {
                lastTransactionStatus = "Balance below threshold. No interest applied.";
            }
        }
    }
}
