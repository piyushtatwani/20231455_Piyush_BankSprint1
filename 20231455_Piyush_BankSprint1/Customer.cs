using System.Collections.Generic;

namespace BankSprint1
{
    public abstract class Customer
    {
        // Private fields for encapsulation
        private int customerNumber;
        private string name;
        private string contactDetails;
        private List<Account> accounts;

        // Public properties
        public int CustomerNumber { get { return customerNumber; } }
        public string Name { get { return name; } }
        public string ContactDetails { get { return contactDetails; } }
        public List<Account> Accounts { get { return accounts; } }

        public Customer(int custNum, string custName, string contact)
        {
            customerNumber = custNum;
            name = custName;
            contactDetails = contact;
            
            // Pre-populate the list with one of each type of account
            accounts = new List<Account>
            {
                new EverydayAccount(this, 0),
                new InvestmentAccount(this, 0),
                new OmniAccount(this, 0)
            };
        }

        // Abstract method to be overridden by child classes to calculate transaction fees
        public abstract double CalculateFee(double baseFee);
    }
}
