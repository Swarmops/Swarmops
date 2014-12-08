using Swarmops.Basic.Types.Financial;

namespace Swarmops.Logic.Financial
{
    public class FinancialAccountRow : BasicFinancialAccountRow
    {
        #region Construction and Creation

        private FinancialAccountRow (BasicFinancialAccountRow basic)
            : base (basic)
        {
        }

        public decimal Amount
        {
            get { return AmountCents/100.0m; }
        }

        public static FinancialAccountRow FromBasic (BasicFinancialAccountRow basic)
        {
            return new FinancialAccountRow (basic);
        }

        #endregion

        public FinancialTransaction Transaction
        {
            get { return FinancialTransaction.FromIdentity (FinancialTransactionId); }
        }
    }
}