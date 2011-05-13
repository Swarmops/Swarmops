using Activizr.Basic.Types;

namespace Activizr.Logic.Financial
{
    public class FinancialAccountRow : BasicFinancialAccountRow
    {
        #region Construction and Creation

        private FinancialAccountRow (BasicFinancialAccountRow basic)
            : base (basic)
        {
        }

        public static FinancialAccountRow FromBasic (BasicFinancialAccountRow basic)
        {
            return new FinancialAccountRow (basic);
        }

        public decimal Amount
        {
            get { return AmountCents/100.0m; }
        }

        #endregion
    }
}