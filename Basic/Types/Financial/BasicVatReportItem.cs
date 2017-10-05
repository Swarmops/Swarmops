using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Common.Enums;
using Swarmops.Common.Interfaces;

namespace Swarmops.Basic.Types.Financial
{
    public class BasicVatReportItem: IHasIdentity
    {
        // Normal ctor

        // Copy ctor

        // Properties

        public int VatReportItemId { get; private set; }

        public int ForeignId { get; private set; }
        public int FinancialTransactionId { get; private set; }
        public FinancialDependencyType DependencyType { get; private set; }

        // Interfaces

        public int Identity { get { return this.VatReportItemId; } }
    }
}
