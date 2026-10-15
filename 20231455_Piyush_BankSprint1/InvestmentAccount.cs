using System;

namespace BankSprint1
{
    public class InvestmentAccount : Account
    {
        private double interestRate = 0.05; // 5% variable interest
        private double failedTransactionFee = 10.0; // Fixed fee

        // Constructor uses the base keyword
        public InvestmentAccount(Customer owner, double initialBalance) 
            : base(owner, "Investment Account", initialBalance)
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
                // No overdraft allowed, charge fixed fee based on customer type
                double fee = AccountOwner.CalculateFee(failedTransactionFee);
                double originalBalance = balance;
                balance -= fee;
                lastTransactionStatus = $"[Investment Account] Insufficient funds. Attempted withdrawal of ${amount:F2} exceeds available balance of ${originalBalance:F2}. A failed transaction fee of ${fee:F2} has been charged.";
                throw new InsufficientFundsException(lastTransactionStatus, accountName, amount, originalBalance);
            }
        }

        public override void CalculateInterest()
        {
            double interest = balance * interestRate;
            balance += interest;
            lastTransactionStatus = $"Interest Applied: ${interest:F2}";
        }
    }
}
