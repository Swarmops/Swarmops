using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swarmops.Basic.Types.Financial;

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

        public int ParentIdentity
        {
            get { return base.ParentFinancialTransactionTagTypeId; }
        }
    }
}
