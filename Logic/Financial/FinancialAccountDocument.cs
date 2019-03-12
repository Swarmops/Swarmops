using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Basic.Types.Financial;
using Swarmops.Common.Enums;
using Swarmops.Database;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Financial
{
    [Serializable]
    public class FinancialAccountDocument: BasicFinancialAccountDocument
    {
        [Obsolete("The parameterless ctor is only here for serialization. Use FromIdentity() or Create().", true)]
        public FinancialAccountDocument()
        {
        }

        private FinancialAccountDocument(BasicFinancialAccountDocument basic) : base(basic)
        {
            // private ctor
        }

        public static FinancialAccountDocument FromBasic(BasicFinancialAccountDocument basic)
        {
            return new FinancialAccountDocument(basic);
        }

        public static FinancialAccountDocument FromIdentity(int financialAccountDocumentId)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetFinancialAccountDocument(financialAccountDocumentId));
        }

        internal static FinancialAccountDocument FromIdentityAggressive(int financialAccountDocumentId)
        {
            return FromBasic(SwarmDb.GetDatabaseForWriting().GetFinancialAccountDocument(financialAccountDocumentId));
        }

        public static FinancialAccountDocument Create(FinancialAccount account, FinancialAccountDocumentType documentType, Person uploader, DateTime concernsStart, DateTime concernsEnd, string rawText)
        {
            return FromIdentityAggressive(
                SwarmDb.GetDatabaseForWriting().CreateFinancialAccountDocument(
                    account.Identity,
                    documentType,
                    uploader == null? 0: uploader.Identity,
                    concernsStart,
                    concernsEnd,
                    rawText
                    ));
        }
    }
}
