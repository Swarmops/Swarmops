using Swarmops.Basic.Types.Financial;
using Swarmops.Database;

namespace Swarmops.Logic.Financial
{
    public class FinancialTransactionTagSet: BasicFinancialTransactionTagSet
    {
        private FinancialTransactionTagSet (BasicFinancialTransactionTagSet basic): base (basic)
        {
            // ctor
        }

        public static FinancialTransactionTagSet FromBasic (BasicFinancialTransactionTagSet basic)
        {
            return new FinancialTransactionTagSet(basic);
        }

        public static FinancialTransactionTagSet FromIdentity (int identity)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetFinancialTransactionTagSet(identity));
        }

        public new TagSetProfitLossType ProfitLossType
        {
            get { return (TagSetProfitLossType) base.ProfitLossType; }
        }
    }

    public enum TagSetProfitLossType
    {
        Unknown = 0,
        /// <summary>
        /// This tag set should only be applied to income.
        /// </summary>
        ProfitOnly = 1,
        /// <summary>
        /// This tag set should only be applied to costs.
        /// </summary>
        LossOnly = 2,
        /// <summary>
        /// This tag set should be applied to both income and costs.
        /// </summary>
        Both = 3
    }
    
}
