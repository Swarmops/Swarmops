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

        public BasicVatReportItem(int vatReportItemId, int vatReportId, int financialTransactionId, int foreignId,
            FinancialDependencyType dependencyType, Int64 turnoverCents, Int64 vatInboundCents, Int64 vatOutboundCents)
        {
            this.VatReportItemId = vatReportItemId;
            this.VatReportId = VatReportId;
            this.FinancialTransactionId = financialTransactionId;
            this.ForeignId = foreignId;
            this.DependencyType = dependencyType;
            this.TurnoverCents = turnoverCents;
            this.VatInboundCents = vatInboundCents;
            this.VatOutboundCents = vatOutboundCents;
        }

        // Copy ctor

        public BasicVatReportItem(BasicVatReportItem original) :
            this(original.VatReportItemId, original.VatReportId, original.FinancialTransactionId, original.ForeignId,
                original.DependencyType, original.TurnoverCents, original.VatInboundCents, original.VatOutboundCents)
        {
            // empty copy ctor
        }

        // Properties

        public int VatReportItemId { get; private set; }
        public int VatReportId { get; private set; }
        public int FinancialTransactionId { get; private set; }
        public int ForeignId { get; private set; }
        public FinancialDependencyType DependencyType { get; private set; }
        public Int64 TurnoverCents { get; private set; }
        public Int64 VatInboundCents { get; private set; }
        public Int64 VatOutboundCents { get; private set; }

        // Interfaces

        public int Identity { get { return this.VatReportItemId; } }
    }
}
