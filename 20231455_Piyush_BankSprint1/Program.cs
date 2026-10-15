using System;
using BankSprint1;

Console.WriteLine("==============================================");
Console.WriteLine("    KIWIBANK SPRINT 2 - SYSTEM RELIABILITY    ");
Console.WriteLine("==============================================\n");

// Create customers
Customer piyush = new BankStaff(1, "Piyush", "piyush@bank.com");
Customer shahzad = new RegularCustomer(2, "Shahzad Khan", "shahzad@email.com");

Console.WriteLine($"Staff Customer: {piyush.Name} (50% Fee Discount)");
Console.WriteLine($"Regular Customer: {shahzad.Name} (Standard Fees)");
Console.WriteLine("----------------------------------------------\n");

// 1. Test Everyday Account (Index 0 - No fees, no overdraft)
Console.WriteLine("--- 1. Testing Everyday Account ---");
Console.WriteLine("Piyush (Staff):");
piyush.Accounts[0].Deposit(100);
try
{
    piyush.Accounts[0].Withdraw(200); // Fails, no fee, triggers InsufficientFundsException
}
catch (BankingException ex)
{
    Console.WriteLine($"[Exception Handled]: {ex.Message}");
}
Console.WriteLine(piyush.Accounts[0].GetAccountInfo());

Console.WriteLine("\nShahzad Khan (Regular):");
shahzad.Accounts[0].Deposit(100);
try
{
    shahzad.Accounts[0].Withdraw(200); // Fails, no fee, triggers InsufficientFundsException
}
catch (BankingException ex)
{
    Console.WriteLine($"[Exception Handled]: {ex.Message}");
}
Console.WriteLine(shahzad.Accounts[0].GetAccountInfo());
Console.WriteLine("\n");

// 2. Test Investment Account (Index 1 - Fixed fee for failed, no overdraft)
Console.WriteLine("--- 2. Testing Investment Account ---");
Console.WriteLine("Piyush (Staff) - 50% fee discount ($5.00 fee):");
piyush.Accounts[1].Deposit(100);
try
{
    piyush.Accounts[1].Withdraw(200); // Fails, triggers $5 fee
}
catch (BankingException ex)
{
    Console.WriteLine($"[Exception Handled]: {ex.Message}");
}
Console.WriteLine(piyush.Accounts[1].GetAccountInfo());

Console.WriteLine("\nShahzad Khan (Regular) - Full fee ($10.00 fee):");
shahzad.Accounts[1].Deposit(100);
try
{
    shahzad.Accounts[1].Withdraw(200); // Fails, triggers $10 fee
}
catch (BankingException ex)
{
    Console.WriteLine($"[Exception Handled]: {ex.Message}");
}
Console.WriteLine(shahzad.Accounts[1].GetAccountInfo());
Console.WriteLine("\n");

// 3. Test Omni Account (Index 2 - Fixed fee for failed, allowed $500 overdraft)
Console.WriteLine("--- 3. Testing Omni Account ---");
Console.WriteLine("Piyush (Staff) - Exceeds $100 balance + $500 overdraft ($5.00 fee):");
piyush.Accounts[2].Deposit(100);
try
{
    piyush.Accounts[2].Withdraw(700); // Fails, triggers $5 fee
}
catch (BankingException ex)
{
    Console.WriteLine($"[Exception Handled]: {ex.Message}");
}
Console.WriteLine(piyush.Accounts[2].GetAccountInfo());

Console.WriteLine("\nShahzad Khan (Regular) - Exceeds $100 balance + $500 overdraft ($10.00 fee):");
shahzad.Accounts[2].Deposit(100);
try
{
    shahzad.Accounts[2].Withdraw(700); // Fails, triggers $10 fee
}
catch (BankingException ex)
{
    Console.WriteLine($"[Exception Handled]: {ex.Message}");
}
Console.WriteLine(shahzad.Accounts[2].GetAccountInfo());

Console.WriteLine("\n==============================================");
Console.WriteLine("All test scenarios executed successfully!");
Console.WriteLine("==============================================");
