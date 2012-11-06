using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Financial
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
            return FromBasic (PirateDb.GetDatabaseForReading().GetFinancialTransactionRow (identity));
        }

        public string AccountName
        {
            get { return Account.Name; }   
        }

        public decimal Amount
        {
            get { return AmountCents/100.0m; }
        }
    }
}