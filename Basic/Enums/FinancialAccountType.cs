using System;
using System.Collections.Generic;
using System.Text;

namespace Swarmops.Basic.Enums
{
    public enum FinancialAccountType
    {
        /// <summary>
        /// Unknown account type.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// An asset, such as a bank account. Balance account.
        /// </summary>
        Asset = 1,
        /// <summary>
        /// A debt, such as unprocessed activist expenses. Balance account.
        /// </summary>
        Debt = 2,
        /// <summary>
        /// An income, any income. Result account. Received money is negative (sign reversed account).
        /// </summary>
        Income = 3,
        /// <summary>
        /// A cost, any cost. Result account. Spent money is positive (sign reversed account).
        /// </summary>
        Cost = 4,
        /// <summary>
        /// Assets and debts - accounts that keep accumulating over years.
        /// </summary>
        Balance = 5,
        /// <summary>
        /// Incomes and costs - accounts that are zeroed at year end. Sign reversed.
        /// </summary>
        Result = 6
    }
}