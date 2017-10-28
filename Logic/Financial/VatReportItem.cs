using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Basic.Types.Financial;
using Swarmops.Common.Enums;
using Swarmops.Common.Interfaces;
using Swarmops.Database;

namespace Swarmops.Logic.Financial
{
    public class VatReportItem: BasicVatReportItem
    {
        #region Creation and Construction

        private VatReportItem(BasicVatReportItem basic) :
            base(basic)
        {
            // private ctor prevents random uncontrolled instantiation
        }

        public static VatReportItem FromBasic(BasicVatReportItem basic)
        {
            // interface to private ctor
            return new VatReportItem(basic);
        }

        public static VatReportItem FromIdentity(int vatReportItemId)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetVatReportItem(vatReportItemId));
        }

        public static VatReportItem FromIdentityAggressive(int vatReportItemId)
        {
            // "Open For Writing" is intentional - it bypasses the lag of replication-to-readonly
            // instances of large-deployment databases and reads at the write source

            return FromBasic(SwarmDb.GetDatabaseForWriting().GetVatReportItem(vatReportItemId));
        }

        public static VatReportItem Create(VatReport report, FinancialTransaction transaction, Int64 turnoverCents,
            Int64 vatInboundCents, Int64 vatOutboundCents)
        {
            // Assumes there's a dependency of some sort

            IHasIdentity foreignObject = transaction.Dependency;
            FinancialDependencyType dependencyType = 
                (foreignObject != null
                ? FinancialTransaction.GetFinancialDependencyType(transaction.Dependency)
                : FinancialDependencyType.Unknown);

            // The transaction dependency is stored for quick lookup; it duplicates information in the database
            // to save an expensive query as a mere optimization.

            int newVatReportItemId = SwarmDb.GetDatabaseForWriting()
                .CreateVatReportItem(report.Identity, transaction.Identity, foreignObject?.Identity ?? 0,
                    dependencyType, turnoverCents, vatInboundCents, vatOutboundCents);

            return FromIdentityAggressive(newVatReportItemId);
        }

        #endregion

        public FinancialTransaction Transaction
        {
            get { return FinancialTransaction.FromIdentity(this.FinancialTransactionId); }
        }
    }
}
