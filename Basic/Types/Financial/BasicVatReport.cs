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

        public BasicVatReport(int vatReportId, int organizationId, string guid, DateTime createdDateTime,
            int yearMonthStart, int monthCount, bool open, Int64 turnoverCents, Int64 vatInboundCents, 
            Int64 vatOutboundCents, int openTransactionId, int closeTransactionId, bool underConstruction)
        {
            this.VatReportId = vatReportId;
            this.OrganizationId = organizationId;
            this.Guid = guid;
            this.CreatedDateTime = createdDateTime;
            this.YearMonthStart = yearMonthStart;
            this.MonthCount = monthCount;
            this.Open = open;
            this.TurnoverCents = turnoverCents;
            this.VatInboundCents = vatInboundCents;
            this.VatOutboundCents = vatOutboundCents;
            this.OpenTransactionId = openTransactionId;
            this.CloseTransactionId = closeTransactionId;
            this.UnderConstruction = underConstruction;
        }

        // Copy ctor

        public BasicVatReport(BasicVatReport original)
            : this(original.Identity, original.OrganizationId, original.Guid, original.CreatedDateTime,
                original.YearMonthStart, original.MonthCount, original.Open, original.TurnoverCents,
                original.VatInboundCents, original.VatOutboundCents, original.OpenTransactionId,
                original.CloseTransactionId, original.UnderConstruction)
        {
            // empty copy ctor
        }

        // Properties

        public int VatReportId { get; private set; }
        public int OrganizationId { get; private set; }
        public string Guid { get; private set; }
        public DateTime CreatedDateTime { get; private set; }
        public int YearMonthStart { get; private set; }
        public int MonthCount { get; private set; }
        public Int64 TurnoverCents { get; protected set; }
        public Int64 VatInboundCents { get; protected set; }
        public Int64 VatOutboundCents { get; protected set; }
        public bool Open { get; protected set; }
        public int OpenTransactionId { get; protected set; }
        public int CloseTransactionId { get; protected set; }
        public bool UnderConstruction { get; protected set; }

        // Interfaces

        public int Identity
        {
            get { return this.VatReportId; }
        }
    }
}
