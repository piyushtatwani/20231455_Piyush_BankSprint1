using System;

namespace BankSprint1
{
    public class BankStaff : Customer
    {
        // Constructor using base keyword
        public BankStaff(int customerNumber, string name, string contactDetails) 
            : base(customerNumber, name, contactDetails)
        {
        }

        public override double CalculateFee(double baseFee)
        {
            // Bank staff receive a 50% discount on transaction fees
            return baseFee * 0.5;
        }
    }
}
