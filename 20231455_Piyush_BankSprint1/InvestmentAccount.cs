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
                // No overdraft allowed, but charge fixed fee based on customer type
                double fee = AccountOwner.CalculateFee(failedTransactionFee);
                balance -= fee;
                lastTransactionStatus = $"Insufficient Funds. Failed Transaction Fee of ${fee} charged.";
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
