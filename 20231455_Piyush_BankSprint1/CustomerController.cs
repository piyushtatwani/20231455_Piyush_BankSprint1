using System;
using System.Collections.Generic;
using System.Linq;

namespace BankSprint1
{
    /// <summary>
    /// Controller class in the MVC architecture.
    /// Manages Customer models and mediates all data requests and operations
    /// between the View (Windows Forms / CLI) and the Model layer.
    /// </summary>
    public class CustomerController
    {
        // Encapsulated data store for all customers
        private readonly List<Customer> customers;

        public CustomerController()
        {
            customers = new List<Customer>();
            SeedInitialData();
        }

        /// <summary>
        /// Pre-populates the controller with initial sample customers.
        /// </summary>
        private void SeedInitialData()
        {
            // Seed sample customers from Sprint 1
            AddCustomer("Shahzad Khan", "shahzad@email.com", isStaff: false);
            AddCustomer("Piyush", "piyush@bank.com", isStaff: true);
        }

        #region CRUD Operations

        /// <summary>
        /// CREATE: Adds a new customer (either BankStaff or RegularCustomer) with auto-generated ID.
        /// </summary>
        public Customer AddCustomer(string name, string contactDetails, bool isStaff)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Customer name cannot be empty.", nameof(name));
            }

            int nextId = customers.Count > 0 ? customers.Max(c => c.CustomerNumber) + 1 : 1;
            Customer newCustomer;

            if (isStaff)
            {
                newCustomer = new BankStaff(nextId, name.Trim(), contactDetails?.Trim() ?? string.Empty);
            }
            else
            {
                newCustomer = new RegularCustomer(nextId, name.Trim(), contactDetails?.Trim() ?? string.Empty);
            }

            customers.Add(newCustomer);
            return newCustomer;
        }

        /// <summary>
        /// READ: Retrieves all customers as a read-only list to prevent direct manipulation by the View.
        /// </summary>
        public IReadOnlyList<Customer> GetAllCustomers()
        {
            return customers.AsReadOnly();
        }

        /// <summary>
        /// READ: Finds a specific customer by their customer number.
        /// </summary>
        public Customer? GetCustomerById(int customerNumber)
        {
            return customers.FirstOrDefault(c => c.CustomerNumber == customerNumber);
        }

        /// <summary>
        /// UPDATE: Updates the name and contact details of an existing customer.
        /// </summary>
        public bool UpdateCustomer(int customerNumber, string newName, string newContactDetails)
        {
            if (string.IsNullOrWhiteSpace(newName))
            {
                throw new ArgumentException("Customer name cannot be empty.", nameof(newName));
            }

            Customer? customer = GetCustomerById(customerNumber);
            if (customer != null)
            {
                customer.UpdateDetails(newName.Trim(), newContactDetails?.Trim() ?? string.Empty);
                return true;
            }

            return false;
        }

        /// <summary>
        /// DELETE: Removes a customer by customer number.
        /// </summary>
        public bool DeleteCustomer(int customerNumber)
        {
            Customer? customer = GetCustomerById(customerNumber);
            if (customer != null)
            {
                customers.Remove(customer);
                return true;
            }

            return false;
        }

        #endregion

        #region Account & Transaction Operations

        /// <summary>
        /// Retrieves a specific account for a customer by account index.
        /// </summary>
        public Account? GetAccount(int customerNumber, int accountIndex)
        {
            Customer? customer = GetCustomerById(customerNumber);
            if (customer != null && accountIndex >= 0 && accountIndex < customer.Accounts.Count)
            {
                return customer.Accounts[accountIndex];
            }
            return null;
        }

        /// <summary>
        /// Performs a deposit onto a customer's specific account with graceful exception handling.
        /// </summary>
        public bool Deposit(int customerNumber, int accountIndex, double amount, out string status)
        {
            Account? acc = GetAccount(customerNumber, accountIndex);
            if (acc != null)
            {
                try
                {
                    acc.Deposit(amount);
                    status = acc.LastTransactionStatus;
                    return true;
                }
                catch (BankingException ex)
                {
                    status = ex.Message;
                    return false;
                }
            }

            status = "Error: Customer or account not found.";
            return false;
        }

        /// <summary>
        /// Performs a withdrawal from a customer's specific account with graceful exception handling.
        /// </summary>
        public bool Withdraw(int customerNumber, int accountIndex, double amount, out string status)
        {
            Account? acc = GetAccount(customerNumber, accountIndex);
            if (acc != null)
            {
                try
                {
                    acc.Withdraw(amount);
                    status = acc.LastTransactionStatus;
                    return true;
                }
                catch (BankingException ex)
                {
                    status = ex.Message;
                    return false;
                }
            }

            status = "Error: Customer or account not found.";
            return false;
        }

        /// <summary>
        /// Triggers interest calculation for a customer's specific account.
        /// </summary>
        public bool CalculateInterest(int customerNumber, int accountIndex, out string status)
        {
            Account? acc = GetAccount(customerNumber, accountIndex);
            if (acc != null)
            {
                acc.CalculateInterest();
                status = acc.LastTransactionStatus;
                return true;
            }

            status = "Error: Customer or account not found.";
            return false;
        }

        #endregion
    }
}
