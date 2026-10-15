using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BankSprint1;

namespace BankSprint2_Tests
{
    [TestClass]
    public class AccountBusinessLogicTests
    {
        private Customer regularCustomer = null!;
        private Customer staffCustomer = null!;

        [TestInitialize]
        public void Setup()
        {
            // Set up test fixtures before each test
            regularCustomer = new RegularCustomer(1, "Regular Test User", "regular@test.com");
            staffCustomer = new BankStaff(2, "Staff Test User", "staff@test.com");
        }

        #region 1. Deposit Tests (Base Account Logic Inherited by Concrete Classes)

        [TestMethod]
        public void Deposit_ValidAmount_IncreasesBalance_AcrossAllAccountTypes()
        {
            // Arrange
            Account everyday = regularCustomer.Accounts[0];
            Account investment = regularCustomer.Accounts[1];
            Account omni = regularCustomer.Accounts[2];

            // Act
            everyday.Deposit(150.00);
            investment.Deposit(250.50);
            omni.Deposit(500.00);

            // Assert
            Assert.AreEqual(150.00, everyday.Balance, 0.001, "Everyday account balance should match deposited amount.");
            Assert.AreEqual(250.50, investment.Balance, 0.001, "Investment account balance should match deposited amount.");
            Assert.AreEqual(500.00, omni.Balance, 0.001, "Omni account balance should match deposited amount.");
            Assert.AreEqual("Deposit Successful", everyday.LastTransactionStatus);
        }

        [TestMethod]
        [ExpectedException(typeof(BankingException))]
        public void Deposit_ZeroAmount_ThrowsBankingException_EdgeCase()
        {
            // Arrange
            Account everyday = regularCustomer.Accounts[0];

            // Act: Edge case attempting to deposit $0.00
            everyday.Deposit(0.00);
        }

        [TestMethod]
        [ExpectedException(typeof(BankingException))]
        public void Deposit_NegativeAmount_ThrowsBankingException_EdgeCase()
        {
            // Arrange
            Account investment = regularCustomer.Accounts[1];

            // Act: Negative deposit should throw
            investment.Deposit(-50.00);
        }

        #endregion

        #region 2. Everyday Account Withdrawal Tests

        [TestMethod]
        public void EverydayAccount_Withdraw_NormalAmount_SuccessfullyDeductsBalance()
        {
            // Arrange
            Account acc = regularCustomer.Accounts[0];
            acc.Deposit(200.00);

            // Act
            acc.Withdraw(75.00);

            // Assert
            Assert.AreEqual(125.00, acc.Balance, 0.001);
            Assert.AreEqual("Withdrawal Successful", acc.LastTransactionStatus);
        }

        [TestMethod]
        public void EverydayAccount_Withdraw_ExactBalanceAmount_ReducesBalanceToZero_EdgeCase()
        {
            // Arrange
            Account acc = regularCustomer.Accounts[0];
            acc.Deposit(100.00);

            // Act: Edge case withdrawing exact balance
            acc.Withdraw(100.00);

            // Assert
            Assert.AreEqual(0.00, acc.Balance, 0.001, "Withdrawing exact balance should leave $0.00.");
            Assert.AreEqual("Withdrawal Successful", acc.LastTransactionStatus);
        }

        [TestMethod]
        [ExpectedException(typeof(BankingException))]
        public void EverydayAccount_Withdraw_ZeroAmount_ThrowsBankingException_EdgeCase()
        {
            // Arrange
            Account acc = regularCustomer.Accounts[0];
            acc.Deposit(100.00);

            // Act: Edge case withdrawing $0
            acc.Withdraw(0.00);
        }

        [TestMethod]
        public void EverydayAccount_Withdraw_ExceedsBalance_ThrowsInsufficientFundsException_NoFeesCharged()
        {
            // Arrange
            Account acc = regularCustomer.Accounts[0];
            acc.Deposit(50.00);

            // Act & Assert
            try
            {
                acc.Withdraw(100.00);
                Assert.Fail("Expected InsufficientFundsException was not thrown.");
            }
            catch (InsufficientFundsException ex)
            {
                // Assert exception details and verify no penalty fee was charged
                Assert.AreEqual(50.00, acc.Balance, 0.001, "Everyday account should NOT charge a fee on failed withdrawal.");
                Assert.AreEqual(100.00, ex.AttemptedAmount);
                Assert.AreEqual(50.00, ex.AvailableFunds);
                Assert.IsTrue(ex.Message.Contains("Everyday accounts do not permit overdraft"), "Message should be tailored to Everyday account.");
            }
        }

        #endregion

        #region 3. Investment Account Tests (Withdrawal, Failed Fees & Interest)

        [TestMethod]
        public void InvestmentAccount_Withdraw_NormalAmount_SuccessfullyDeductsBalance()
        {
            // Arrange
            Account acc = regularCustomer.Accounts[1];
            acc.Deposit(300.00);

            // Act
            acc.Withdraw(100.00);

            // Assert
            Assert.AreEqual(200.00, acc.Balance, 0.001);
            Assert.AreEqual("Withdrawal Successful", acc.LastTransactionStatus);
        }

        [TestMethod]
        public void InvestmentAccount_Withdraw_ExactBalance_ReducesBalanceToZero_EdgeCase()
        {
            // Arrange
            Account acc = regularCustomer.Accounts[1];
            acc.Deposit(150.00);

            // Act: Edge case exact balance
            acc.Withdraw(150.00);

            // Assert
            Assert.AreEqual(0.00, acc.Balance, 0.001);
            Assert.AreEqual("Withdrawal Successful", acc.LastTransactionStatus);
        }

        [TestMethod]
        public void InvestmentAccount_Withdraw_Failed_RegularCustomer_ChargesFull10DollarFee_AndThrowsException()
        {
            // Arrange: Regular customer with $100
            Account acc = regularCustomer.Accounts[1];
            acc.Deposit(100.00);

            // Act & Assert: Attempting to withdraw $200 (fails, incurs full $10 fee)
            try
            {
                acc.Withdraw(200.00);
                Assert.Fail("Expected InsufficientFundsException was not thrown.");
            }
            catch (InsufficientFundsException ex)
            {
                // Balance should be deducted by $10 fee ($100 - $10 = $90)
                Assert.AreEqual(90.00, acc.Balance, 0.001, "Regular customer should be charged full $10 fee.");
                Assert.IsTrue(ex.Message.Contains("failed transaction fee of $10.00 has been charged"));
            }
        }

        [TestMethod]
        public void InvestmentAccount_Withdraw_Failed_BankStaff_ChargesDiscounted5DollarFee_AndThrowsException()
        {
            // Arrange: Bank Staff customer with $100 (50% fee discount)
            Account acc = staffCustomer.Accounts[1];
            acc.Deposit(100.00);

            // Act & Assert: Attempting to withdraw $200 (fails, incurs 50% discount fee = $5)
            try
            {
                acc.Withdraw(200.00);
                Assert.Fail("Expected InsufficientFundsException was not thrown.");
            }
            catch (InsufficientFundsException ex)
            {
                // Balance should be deducted by $5 fee ($100 - $5 = $95)
                Assert.AreEqual(95.00, acc.Balance, 0.001, "Bank Staff customer should receive 50% fee discount ($5.00).");
                Assert.IsTrue(ex.Message.Contains("failed transaction fee of $5.00 has been charged"));
            }
        }

        [TestMethod]
        public void InvestmentAccount_CalculateInterest_AppliesFivePercentInterestOnTotalBalance()
        {
            // Arrange: $1,000 balance -> 5% interest = $50.00
            Account acc = regularCustomer.Accounts[1];
            acc.Deposit(1000.00);

            // Act
            acc.CalculateInterest();

            // Assert
            Assert.AreEqual(1050.00, acc.Balance, 0.001, "Investment account should apply 5% interest.");
            Assert.AreEqual("Interest Applied: $50.00", acc.LastTransactionStatus);
        }

        #endregion

        #region 4. Omni Account Tests (Withdrawal, Overdraft Limit Edge Cases & Threshold Interest)

        [TestMethod]
        public void OmniAccount_Withdraw_WithinPositiveBalance_Success()
        {
            // Arrange
            Account acc = regularCustomer.Accounts[2];
            acc.Deposit(200.00);

            // Act
            acc.Withdraw(50.00);

            // Assert
            Assert.AreEqual(150.00, acc.Balance, 0.001);
            Assert.AreEqual("Withdrawal Successful", acc.LastTransactionStatus);
        }

        [TestMethod]
        public void OmniAccount_Withdraw_Within500Overdraft_Success()
        {
            // Arrange: Balance $100. Allowed to withdraw up to $100 + $500 = $600.
            Account acc = regularCustomer.Accounts[2];
            acc.Deposit(100.00);

            // Act: Withdraw $400 (exceeds balance, utilizes $300 of overdraft)
            acc.Withdraw(400.00);

            // Assert
            Assert.AreEqual(-300.00, acc.Balance, 0.001, "Omni account balance should reflect overdraft usage.");
            Assert.AreEqual("Withdrawal Successful", acc.LastTransactionStatus);
        }

        [TestMethod]
        public void OmniAccount_Withdraw_ExactOverdraftLimit_BalanceBecomesMinus500_EdgeCase()
        {
            // Arrange: $100 balance + $500 overdraft limit = exact $600 withdrawal
            Account acc = regularCustomer.Accounts[2];
            acc.Deposit(100.00);

            // Act: Edge case withdrawing exact max allowance
            acc.Withdraw(600.00);

            // Assert
            Assert.AreEqual(-500.00, acc.Balance, 0.001, "Balance should be exactly -$500.00.");
            Assert.AreEqual("Withdrawal Successful", acc.LastTransactionStatus);
        }

        [TestMethod]
        public void OmniAccount_Withdraw_ExceedsOverdraftLimitByOneCent_ThrowsOverdraftLimitExceededException_EdgeCase()
        {
            // Arrange: Balance $100. Overdraft limit $500. Max permitted = $600.00.
            // Edge case: Exceed overdraft limit by exactly $0.01 ($600.01)
            Account acc = regularCustomer.Accounts[2];
            acc.Deposit(100.00);

            // Act & Assert
            try
            {
                acc.Withdraw(600.01);
                Assert.Fail("Expected OverdraftLimitExceededException was not thrown for $0.01 boundary violation.");
            }
            catch (OverdraftLimitExceededException ex)
            {
                // Verify full regular customer fee of $10 was charged ($100 - $10 = $90)
                Assert.AreEqual(90.00, acc.Balance, 0.001, "Should charge $10 fee on overdraft limit breach.");
                Assert.AreEqual(600.01, ex.AttemptedAmount);
                Assert.AreEqual(500.00, ex.OverdraftLimit);
                Assert.IsTrue(ex.Message.Contains("Overdraft limit exceeded"), "Message should be tailored to Omni account overdraft.");
            }
        }

        [TestMethod]
        public void OmniAccount_Withdraw_Failed_BankStaff_ChargesDiscounted5DollarFee()
        {
            // Arrange: Bank Staff with $100 balance.
            Account acc = staffCustomer.Accounts[2];
            acc.Deposit(100.00);

            // Act & Assert: Attempt to withdraw $700 (exceeds $600 limit)
            try
            {
                acc.Withdraw(700.00);
                Assert.Fail("Expected OverdraftLimitExceededException was not thrown.");
            }
            catch (OverdraftLimitExceededException)
            {
                // 50% discount fee ($5.00) deducted: $100 - $5 = $95
                Assert.AreEqual(95.00, acc.Balance, 0.001, "Bank Staff should be charged discounted $5.00 fee.");
            }
        }

        [TestMethod]
        public void OmniAccount_CalculateInterest_BalanceBelow1000Threshold_AppliesZeroInterest_EdgeCase()
        {
            // Arrange: $1,000 exact threshold boundary -> No interest
            Account acc = regularCustomer.Accounts[2];
            acc.Deposit(1000.00);

            // Act
            acc.CalculateInterest();

            // Assert
            Assert.AreEqual(1000.00, acc.Balance, 0.001, "No interest should be applied at or below $1000 threshold.");
            Assert.AreEqual("Balance below threshold. No interest applied.", acc.LastTransactionStatus);
        }

        [TestMethod]
        public void OmniAccount_CalculateInterest_BalanceAbove1000Threshold_AppliesFourPercentOnlyToSurplus()
        {
            // Arrange: Balance $2,000.
            // Surplus exceeding $1,000 threshold = $1,000.
            // 4% of $1,000 = $40.00 interest.
            // New balance = $2,040.00.
            Account acc = regularCustomer.Accounts[2];
            acc.Deposit(2000.00);

            // Act
            acc.CalculateInterest();

            // Assert
            Assert.AreEqual(2040.00, acc.Balance, 0.001, "Omni interest must apply 4% only to the amount exceeding $1000.");
            Assert.AreEqual("Interest Applied: $40.00", acc.LastTransactionStatus);
        }

        #endregion

        #region 5. Controller & Graceful Exception Handling Tests

        [TestMethod]
        public void Controller_AddUpdateDeleteCustomer_Succeeds()
        {
            // Arrange
            CustomerController controller = new CustomerController();

            // Act: Create
            Customer created = controller.AddCustomer("Test Client", "client@nz.com", isStaff: false);
            Assert.IsNotNull(created);
            Assert.IsTrue(created.CustomerNumber > 0);

            // Act: Read
            Customer? fetched = controller.GetCustomerById(created.CustomerNumber);
            Assert.IsNotNull(fetched);
            Assert.AreEqual("Test Client", fetched.Name);

            // Act: Update
            bool updated = controller.UpdateCustomer(created.CustomerNumber, "Updated Client", "updated@nz.com");
            Assert.IsTrue(updated);
            Assert.AreEqual("Updated Client", fetched.Name);

            // Act: Delete
            bool deleted = controller.DeleteCustomer(created.CustomerNumber);
            Assert.IsTrue(deleted);
            Assert.IsNull(controller.GetCustomerById(created.CustomerNumber));
        }

        [TestMethod]
        public void Controller_Withdraw_GracefullyCatchesException_ReturnsFalseAndUserFriendlyMessage()
        {
            // Arrange
            CustomerController controller = new CustomerController();
            var customers = controller.GetAllCustomers();
            int custId = customers[0].CustomerNumber; // Regular customer Shahzad Khan

            // Act: Everyday account is index 0 (initial balance = $0). Attempt withdrawal of $50.
            bool success = controller.Withdraw(custId, 0, 50.00, out string statusMessage);

            // Assert: Controller must NOT crash and return false with the informative message
            Assert.IsFalse(success, "Controller should return false on failed withdrawal.");
            Assert.IsTrue(statusMessage.Contains("Insufficient funds"), "Status message should contain informative exception text.");
        }

        #endregion
    }
}
