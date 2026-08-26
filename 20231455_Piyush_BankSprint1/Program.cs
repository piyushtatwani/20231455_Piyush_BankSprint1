using System;
using BankSprint1;

// Create customers
Customer piyush = new BankStaff(1, "Piyush", "piyush@bank.com");
Customer shahzad = new RegularCustomer(2, "Shahzad Khan", "shahzad@email.com");

Console.WriteLine("==============================================");
Console.WriteLine($"Staff Customer: {piyush.Name}");
Console.WriteLine($"Regular Customer: {shahzad.Name}");
Console.WriteLine("==============================================\n");

// Test Everyday Account (Index 0 - No fees, no overdraft)
Console.WriteLine("--- Testing Everyday Account ---");
Console.WriteLine("Piyush (Staff):");
piyush.Accounts[0].Deposit(100);
piyush.Accounts[0].Withdraw(200); // Fails, no fee
Console.WriteLine(piyush.Accounts[0].GetAccountInfo());

Console.WriteLine("\nShahzad Khan (Regular):");
shahzad.Accounts[0].Deposit(100);
shahzad.Accounts[0].Withdraw(200); // Fails, no fee
Console.WriteLine(shahzad.Accounts[0].GetAccountInfo());
Console.WriteLine("\n");


// Test Investment Account (Index 1 - Fixed fee for failed, no overdraft)
Console.WriteLine("--- Testing Investment Account ---");
Console.WriteLine("Piyush (Staff) - 50% fee discount:");
piyush.Accounts[1].Deposit(100);
piyush.Accounts[1].Withdraw(200); // Fails, triggers $5 fee
Console.WriteLine(piyush.Accounts[1].GetAccountInfo());

Console.WriteLine("\nShahzad Khan (Regular) - Full fee:");
shahzad.Accounts[1].Deposit(100);
shahzad.Accounts[1].Withdraw(200); // Fails, triggers $10 fee
Console.WriteLine(shahzad.Accounts[1].GetAccountInfo());
Console.WriteLine("\n");


// Test Omni Account (Index 2 - Fixed fee for failed, allowed $500 overdraft)
Console.WriteLine("--- Testing Omni Account ---");
Console.WriteLine("Piyush (Staff) - 50% fee discount:");
piyush.Accounts[2].Deposit(100);
piyush.Accounts[2].Withdraw(700); // Fails (Exceeds $100 balance + $500 overdraft), triggers $5 fee
Console.WriteLine(piyush.Accounts[2].GetAccountInfo());

Console.WriteLine("\nShahzad Khan (Regular) - Full fee:");
shahzad.Accounts[2].Deposit(100);
shahzad.Accounts[2].Withdraw(700); // Fails (Exceeds $100 balance + $500 overdraft), triggers $10 fee
Console.WriteLine(shahzad.Accounts[2].GetAccountInfo());
