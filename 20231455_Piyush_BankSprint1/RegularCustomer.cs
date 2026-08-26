using System;

namespace BankSprint1
{
    public class RegularCustomer : Customer
    {
        // Constructor using base keyword
        public RegularCustomer(int customerNumber, string name, string contactDetails) 
            : base(customerNumber, name, contactDetails)
        {
        }

        public override double CalculateFee(double baseFee)
        {
            // Regular customers pay the full transaction fee
            return baseFee;
        }
    }
}
