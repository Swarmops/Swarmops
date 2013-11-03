using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swarmops.Basic.Types.Financial;
using Swarmops.Database;

namespace Swarmops.Logic.Financial
{
    public class FinancialTransactionTagType: BasicFinancialTransactionTagType
    {
        private FinancialTransactionTagType (BasicFinancialTransactionTagType basicTagType): base (basicTagType)
        {
            // private ctor
        }

        public static FinancialTransactionTagType FromBasic (BasicFinancialTransactionTagType basic)
        {
            return new FinancialTransactionTagType(basic);
        }

        public static FinancialTransactionTagType FromIdentity (int identity)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetFinancialTransactionTagType(identity));
        }

        public int ParentIdentity
        {
            get { return base.ParentFinancialTransactionTagTypeId; }
        }
    }
}
