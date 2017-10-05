using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Common.Interfaces;

namespace Swarmops.Basic.Types.Financial
{
    public class BasicVatReport: IHasIdentity
    {
        // Basic ctor



        // Copy ctor



        // Properties

        public int VatReportId { get; private set; }
        public int OrganizationId { get; private set; }
        public string Guid { get; private set; }
        public int YearMonth { get; private set; }
        public int MonthCount { get; private set; }
        public Int64 TurnoverCents { get; protected set; }
        public Int64 VatOutboundCents { get; protected set; }
        public Int64 VatInboundCents { get; protected set; }
        public bool Open { get; protected set; }
        public bool UnderConstruction { get; protected set; }

        // Interfaces

        public int Identity
        {
            get { return this.VatReportId; }
        }
    }
}
