using System;
using System.Drawing;
using System.Windows.Forms;
using BankSprint1;

namespace TASK3_GUI
{
    /// <summary>
    /// View class in the MVC architecture.
    /// Handles ONLY user input and presentation.
    /// Does not store or directly manipulate customer data collections.
    /// All data queries and state changes are delegated to CustomerController.
    /// </summary>
    public partial class Form1 : Form
    {
        // Reference to the Controller - Form does NOT store List<Customer> directly
        private readonly CustomerController controller;

        // Helper item for populating the customer dropdown without storing full customer entity list in View
        private class CustomerComboItem
        {
            public int CustomerNumber { get; }
            public string DisplayText { get; }

            public CustomerComboItem(int id, string display)
            {
                CustomerNumber = id;
                DisplayText = display;
            }

            public override string ToString() => DisplayText;
        }

        public Form1()
        {
            InitializeComponent();

            // Initialize the Controller
            controller = new CustomerController();

            // Populate account types dropdown
            cmbAccounts.Items.Add("Everyday Account (No Overdraft, No Fees)");
            cmbAccounts.Items.Add("Investment Account (5% Interest, $10 Failed Fee)");
            cmbAccounts.Items.Add("Omni Account ($500 Overdraft, 4% Interest > $1000)");
            cmbAccounts.SelectedIndex = 0;

            // Load customers from controller into UI
            RefreshCustomerList();
            LogActivity("Application initialized. Controller loaded with seed data.");
        }

        #region Customer Management (View -> Controller Delegation)

        /// <summary>
        /// Refreshes the customer selection dropdown using data requested from the Controller.
        /// The View does NOT retain the customer list.
        /// </summary>
        private void RefreshCustomerList(int? selectCustomerId = null)
        {
            cmbCustomers.Items.Clear();

            var customerList = controller.GetAllCustomers();
            int selectedIndex = -1;

            for (int i = 0; i < customerList.Count; i++)
            {
                var customer = customerList[i];
                cmbCustomers.Items.Add(new CustomerComboItem(customer.CustomerNumber, customer.ToString()));

                if (selectCustomerId.HasValue && customer.CustomerNumber == selectCustomerId.Value)
                {
                    selectedIndex = i;
                }
            }

            if (cmbCustomers.Items.Count > 0)
            {
                cmbCustomers.SelectedIndex = selectedIndex >= 0 ? selectedIndex : 0;
            }
            else
            {
                ClearCustomerFields();
                UpdateAccountDisplay();
            }
        }

        private int? GetSelectedCustomerId()
        {
            if (cmbCustomers.SelectedItem is CustomerComboItem item)
            {
                return item.CustomerNumber;
            }
            return null;
        }

        private void cmbCustomers_SelectedIndexChanged(object sender, EventArgs e)
        {
            int? id = GetSelectedCustomerId();
            if (id.HasValue)
            {
                var customer = controller.GetCustomerById(id.Value);
                if (customer != null)
                {
                    lblCustomerIdValue.Text = customer.CustomerNumber.ToString();
                    txtName.Text = customer.Name;
                    txtContact.Text = customer.ContactDetails;
                    chkIsStaff.Checked = customer is BankStaff;
                }
            }
            UpdateAccountDisplay();
        }

        private void btnAddCustomer_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string contact = txtContact.Text.Trim();
            bool isStaff = chkIsStaff.Checked;

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Please enter a customer name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Delegate creation to Controller
                var newCustomer = controller.AddCustomer(name, contact, isStaff);
                LogActivity($"Customer Added: {newCustomer}");
                RefreshCustomerList(newCustomer.CustomerNumber);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to add customer: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdateCustomer_Click(object sender, EventArgs e)
        {
            int? id = GetSelectedCustomerId();
            if (!id.HasValue)
            {
                MessageBox.Show("Please select a customer to update.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string name = txtName.Text.Trim();
            string contact = txtContact.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Customer name cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Delegate update to Controller
            bool success = controller.UpdateCustomer(id.Value, name, contact);
            if (success)
            {
                LogActivity($"Customer [ID: {id.Value}] updated: Name='{name}', Contact='{contact}'.");
                RefreshCustomerList(id.Value);
            }
            else
            {
                MessageBox.Show("Failed to update customer.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeleteCustomer_Click(object sender, EventArgs e)
        {
            int? id = GetSelectedCustomerId();
            if (!id.HasValue)
            {
                MessageBox.Show("Please select a customer to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirmResult = MessageBox.Show(
                $"Are you sure you want to delete Customer #{id.Value}?",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmResult == DialogResult.Yes)
            {
                // Delegate deletion to Controller
                bool success = controller.DeleteCustomer(id.Value);
                if (success)
                {
                    LogActivity($"Customer [ID: {id.Value}] successfully deleted.");
                    RefreshCustomerList();
                }
                else
                {
                    MessageBox.Show("Customer could not be deleted.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnClearCustomer_Click(object sender, EventArgs e)
        {
            ClearCustomerFields();
            cmbCustomers.SelectedIndex = -1;
            UpdateAccountDisplay();
        }

        private void ClearCustomerFields()
        {
            lblCustomerIdValue.Text = "(New)";
            txtName.Clear();
            txtContact.Clear();
            chkIsStaff.Checked = false;
        }

        #endregion

        #region Account & Financial Operations (View -> Controller Delegation)

        private void cmbAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAccountDisplay();
        }

        private void UpdateAccountDisplay()
        {
            int? customerId = GetSelectedCustomerId();
            int accountIndex = cmbAccounts.SelectedIndex;

            if (customerId.HasValue && accountIndex >= 0)
            {
                // Request account information from Controller
                var account = controller.GetAccount(customerId.Value, accountIndex);
                if (account != null)
                {
                    lblBalance.Text = $"Current Balance: ${account.Balance:F2}";
                    return;
                }
            }

            lblBalance.Text = "Current Balance: $0.00";
        }

        private void btnDeposit_Click(object sender, EventArgs e)
        {
            int? customerId = GetSelectedCustomerId();
            if (!customerId.HasValue)
            {
                MessageBox.Show("Please select a customer first.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!double.TryParse(txtAmount.Text, out double amount) || amount <= 0)
            {
                MessageBox.Show("Please enter a valid positive numeric amount.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int accountIndex = cmbAccounts.SelectedIndex;
            // Delegate Deposit transaction to Controller
            bool success = controller.Deposit(customerId.Value, accountIndex, amount, out string status);

            LogActivity($"Deposit ${amount:F2} on Account [{cmbAccounts.Text.Split(' ')[0]}] -> {status}");
            UpdateAccountDisplay();
            txtAmount.Clear();
        }

        private void btnWithdraw_Click(object sender, EventArgs e)
        {
            int? customerId = GetSelectedCustomerId();
            if (!customerId.HasValue)
            {
                MessageBox.Show("Please select a customer first.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!double.TryParse(txtAmount.Text, out double amount) || amount <= 0)
            {
                MessageBox.Show("Please enter a valid positive numeric amount.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int accountIndex = cmbAccounts.SelectedIndex;
            // Delegate Withdraw transaction to Controller
            bool success = controller.Withdraw(customerId.Value, accountIndex, amount, out string status);

            LogActivity($"Withdraw ${amount:F2} from Account [{cmbAccounts.Text.Split(' ')[0]}] -> {status}");
            UpdateAccountDisplay();
            txtAmount.Clear();
        }

        private void btnInterest_Click(object sender, EventArgs e)
        {
            int? customerId = GetSelectedCustomerId();
            if (!customerId.HasValue)
            {
                MessageBox.Show("Please select a customer first.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int accountIndex = cmbAccounts.SelectedIndex;
            // Delegate CalculateInterest transaction to Controller
            bool success = controller.CalculateInterest(customerId.Value, accountIndex, out string status);

            LogActivity($"Interest Calculation on [{cmbAccounts.Text.Split(' ')[0]}] -> {status}");
            UpdateAccountDisplay();
        }

        private void btnClearHistory_Click(object sender, EventArgs e)
        {
            lstHistory.Items.Clear();
            LogActivity("Transaction history cleared.");
        }

        private void LogActivity(string message)
        {
            string timeStamp = DateTime.Now.ToString("HH:mm:ss");
            lstHistory.Items.Add($"[{timeStamp}] {message}");
            lstHistory.TopIndex = Math.Max(lstHistory.Items.Count - 1, 0);
        }

        #endregion
    }
}
