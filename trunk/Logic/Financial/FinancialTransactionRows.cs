using System;
using System.Collections.Generic;
using Activizr.Basic.Enums;
using Activizr.Basic.Types;

namespace Activizr.Logic.Financial
{
    public class FinancialTransactionRows : List<FinancialTransactionRow>
    {
        [Obsolete("Do not use. Use AmountCentsTotal.", true)]
        public decimal AmountTotal
        {
            get
            {
                decimal result = 0.0m;

                foreach (FinancialTransactionRow row in this)
                {
                    result += row.Amount;
                }
                return result;
            }
        }

        public Int64 AmountCentsTotal
        {
            get
            {
                Int64 result = 0;

                foreach (FinancialTransactionRow row in this)
                {
                    result += row.AmountCents;
                }
                return result;
            }
        }

        [Obsolete("Do not use. Use Int64 BalanceCentsDelta.", true)]
        public decimal BalanceDelta
        {
            get
            {
                decimal result = 0.0m;

                foreach (FinancialTransactionRow row in this)
                {
                    if (row.Account.AccountType == FinancialAccountType.Asset || row.Account.AccountType == FinancialAccountType.Debt)
                    {
                        result += row.Amount;
                    }
                }
                return result;
            }
        }

        public decimal BalanceCentsDelta
        {
            get
            {
                Int64 result = 0;

                foreach (FinancialTransactionRow row in this)
                {
                    if (row.Account.AccountType == FinancialAccountType.Asset || row.Account.AccountType == FinancialAccountType.Debt)
                    {
                        result += row.AmountCents;
                    }
                }
                return result;
            }
        }

        public static FinancialTransactionRows FromArray(BasicFinancialTransactionRow[] basicArray)
        {
            var result = new FinancialTransactionRows {Capacity = (basicArray.Length*11/10)};

            foreach (BasicFinancialTransactionRow basic in basicArray)
            {
                result.Add (FinancialTransactionRow.FromBasic (basic));
            }

            return result;
        }
    }
}