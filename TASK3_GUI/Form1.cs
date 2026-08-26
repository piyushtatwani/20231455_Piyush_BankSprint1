using System;
using System.Drawing;
using System.Windows.Forms;
using BankSprint1;

namespace TASK3_GUI
{
    public partial class Form1 : Form
    {
        private Customer currentUser;
        private Account selectedAccount;

        public Form1()
        {
            InitializeComponent();
            
            // Initialize dummy user as Regular Customer by default
            currentUser = new RegularCustomer(1, "Shahzad Khan", "regular@kiwibank.co.nz");
            
            cmbAccounts.Items.Add("Everyday Account");
            cmbAccounts.Items.Add("Investment Account");
            cmbAccounts.Items.Add("Omni Account");
            cmbAccounts.SelectedIndex = 0; // Default selection
        }

        private void chkIsStaff_CheckedChanged(object sender, EventArgs e)
        {
            // Switch user type based on checkbox
            if (chkIsStaff.Checked)
            {
                currentUser = new BankStaff(1, "Piyush", "staff@kiwibank.co.nz");
                LogTransaction("Switched profile to: Bank Staff (50% Fee Discount)");
            }
            else
            {
                currentUser = new RegularCustomer(1, "Shahzad Khan", "regular@kiwibank.co.nz");
                LogTransaction("Switched profile to: Regular Customer (Full Fees)");
            }
            
            // Refresh selected account reference since we created a new Customer
            selectedAccount = currentUser.Accounts[cmbAccounts.SelectedIndex];
            UpdateUI();
        }

        private void cmbAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedAccount = currentUser.Accounts[cmbAccounts.SelectedIndex];
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (selectedAccount != null)
            {
                lblBalance.Text = $"Current Balance: ${selectedAccount.Balance:F2}";
            }
        }

        private void btnDeposit_Click(object sender, EventArgs e)
        {
            if (double.TryParse(txtAmount.Text, out double amount))
            {
                selectedAccount.Deposit(amount);
                LogTransaction($"Deposit ${amount:F2} -> {selectedAccount.LastTransactionStatus}");
                UpdateUI();
                txtAmount.Clear();
            }
            else
            {
                MessageBox.Show("Please enter a valid numeric amount.", "Invalid Input");
            }
        }

        private void btnWithdraw_Click(object sender, EventArgs e)
        {
            if (double.TryParse(txtAmount.Text, out double amount))
            {
                selectedAccount.Withdraw(amount);
                LogTransaction($"Withdraw ${amount:F2} -> {selectedAccount.LastTransactionStatus}");
                UpdateUI();
                txtAmount.Clear();
            }
            else
            {
                MessageBox.Show("Please enter a valid numeric amount.", "Invalid Input");
            }
        }

        private void btnInterest_Click(object sender, EventArgs e)
        {
            selectedAccount.CalculateInterest();
            LogTransaction($"Interest Check -> {selectedAccount.LastTransactionStatus}");
            UpdateUI();
        }

        private void LogTransaction(string message)
        {
            lstHistory.Items.Add(message);
            
            // Auto-scroll to bottom
            lstHistory.TopIndex = Math.Max(lstHistory.Items.Count - 1, 0);
        }
    }
}
