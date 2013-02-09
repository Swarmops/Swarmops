using System;
using Swarmops.Basic.Types;
using Swarmops.Database;

namespace Swarmops.Logic.Financial
{
    public class FinancialTransactionRow : BasicFinancialTransactionRow
    {
        private FinancialAccount account;
        private FinancialTransaction transaction;

        private FinancialTransactionRow (BasicFinancialTransactionRow basic)
            : base (basic)
        {
        }

        public FinancialTransaction Transaction
        {
            get
            {
                if (this.transaction == null)
                {
                    this.transaction = FinancialTransaction.FromIdentity (base.FinancialTransactionId);
                }

                return this.transaction;
            }
        }

        public FinancialAccount Account
        {
            get
            {
                if (this.account == null)
                {
                    this.account = FinancialAccount.FromIdentity (base.FinancialAccountId);
                }

                return this.account;
            }
        }

        public static FinancialTransactionRow FromBasic (BasicFinancialTransactionRow basic)
        {
            return new FinancialTransactionRow (basic);
        }

        public static FinancialTransactionRow FromIdentity (int identity)
        {
            return FromBasic (SwarmDb.GetDatabaseForReading().GetFinancialTransactionRow (identity));
        }

        public string AccountName
        {
            get { return Account.Name; }   
        }

        [Obsolete ("Do not use. Use Int64 AmountCents.", true)]
        public decimal Amount
        {
            get { return AmountCents/100.0m; }
        }
    }
}