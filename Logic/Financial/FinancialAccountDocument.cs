using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Basic.Types.Financial;

namespace Swarmops.Logic.Financial
{
    [Serializable]
    public class FinancialAccountDocument: BasicFinancialAccountDocument
    {
        [Obsolete("The parameterless ctor is only here for serialization. Use FromIdentity() or Create().", true)]
        public FinancialAccountDocument()
        {
        }
    }
}
