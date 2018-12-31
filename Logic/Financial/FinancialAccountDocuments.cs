using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Basic.Types.Financial;
using Swarmops.Common.Enums;
using Swarmops.Database;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Financial
{
    public class FinancialAccountDocuments: PluralBase<FinancialAccountDocuments,FinancialAccountDocument,BasicFinancialAccountDocument>
    {
        public static FinancialAccountDocuments ForAccount(FinancialAccount account, FinancialAccountDocumentType typeRequested = FinancialAccountDocumentType.Unknown)
        {
            FinancialAccountDocuments result = FromArray(SwarmDb.GetDatabaseForReading().GetFinancialAccountDocuments(account));
            result.Sort(new FinancialAccountDocumentsSorterByTypeThenDate());

            if (typeRequested != FinancialAccountDocumentType.Unknown)
            {
                return (FinancialAccountDocuments) result.Where(doc => doc.Type == typeRequested);
            }

            return result;
        }

    }

    internal class FinancialAccountDocumentsSorterByTypeThenDate : IComparer<FinancialAccountDocument>
    {
        public int Compare(FinancialAccountDocument a, FinancialAccountDocument b)
        {
            if (a.Type != b.Type)
            {
                return a.Type.CompareTo(b.Type);
            }

            return (a.ConcernsPeriodStart.CompareTo(b.ConcernsPeriodStart));
        }
    }
}
