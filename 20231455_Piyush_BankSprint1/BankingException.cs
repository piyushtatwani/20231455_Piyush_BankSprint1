using System;

namespace BankSprint1
{
    /// <summary>
    /// Enumeration of specific banking error categories.
    /// </summary>
    public enum BankingErrorType
    {
        GeneralError,
        InvalidAmount,
        InsufficientFunds,
        OverdraftExceeded
    }

    /// <summary>
    /// Base custom exception for all financial and banking operation failures.
    /// </summary>
    public class BankingException : Exception
    {
        public BankingErrorType ErrorType { get; }
        public string AccountName { get; }

        public BankingException(string message, string accountName = "", BankingErrorType errorType = BankingErrorType.GeneralError)
            : base(message)
        {
            AccountName = accountName;
            ErrorType = errorType;
        }

        public BankingException(string message, Exception innerException, string accountName = "", BankingErrorType errorType = BankingErrorType.GeneralError)
            : base(message, innerException)
        {
            AccountName = accountName;
            ErrorType = errorType;
        }
    }

    /// <summary>
    /// Exception thrown when an attempted withdrawal exceeds available balance without overdraft.
    /// </summary>
    public class InsufficientFundsException : BankingException
    {
        public double AttemptedAmount { get; }
        public double AvailableFunds { get; }

        public InsufficientFundsException(string message, string accountName, double attemptedAmount, double availableFunds)
            : base(message, accountName, BankingErrorType.InsufficientFunds)
        {
            AttemptedAmount = attemptedAmount;
            AvailableFunds = availableFunds;
        }
    }

    /// <summary>
    /// Exception thrown when a withdrawal exceeds an account's authorized overdraft limit.
    /// </summary>
    public class OverdraftLimitExceededException : InsufficientFundsException
    {
        public double OverdraftLimit { get; }

        public OverdraftLimitExceededException(string message, string accountName, double attemptedAmount, double availableFunds, double overdraftLimit)
            : base(message, accountName, attemptedAmount, availableFunds)
        {
            OverdraftLimit = overdraftLimit;
        }
    }
}
