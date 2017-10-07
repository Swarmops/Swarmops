using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Basic.Types.Financial;
using Swarmops.Common.Interfaces;
using Swarmops.Database;

namespace Swarmops.Logic.Financial
{
    public class VatReport: BasicVatReport
    {
        #region Creation and Construction

        private VatReport(BasicVatReport basic): base (basic)
        {
            // private ctor prevents random instantiation
        }

        public static VatReport FromBasic(BasicVatReport basic)
        {
            // Interface to private ctor
            return new VatReport(basic);
        }

        public static VatReport FromIdentity(int vatReportId)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetVatReport(vatReportId));
        }

        protected static VatReport FromIdentityAggressive(int vatReportId)
        {
            // "Open For Writing" is intentional - it bypasses the lag of replication-to-readonly
            // instances of large-deployment databases and reads at the write source

            return FromBasic(SwarmDb.GetDatabaseForWriting().GetVatReport(vatReportId));
        }

        #endregion

        public void AddItem(FinancialTransaction transaction, Int64 turnoverCents,
            Int64 vatInboundCents, Int64 vatOutboundCents)
        {
            VatReportItem newItem = VatReportItem.Create(this, transaction, turnoverCents, vatInboundCents,
                vatOutboundCents);
        }

        public void Release()
        {
            // if we're not under construction, throw exception

            if (!this.UnderConstruction)
            {
                throw new InvalidOperationException("VAT report is already released");
            }

            // Releasing calculates sums of turnover, VAT inbound, outbound across items into report record

            SwarmDb.GetDatabaseForWriting().SetVatReportReleased(this.Identity);
            base.UnderConstruction = false;
        }

        public VatReportItems Items
        {
            get { return VatReportItems.ForReport(this); }
        }
    }
}
