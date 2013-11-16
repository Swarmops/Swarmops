using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swarmops.Basic.Types.Financial;
using Swarmops.Database;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Financial
{
    public class FinancialTransactionTagTypes: PluralBase<FinancialTransactionTagTypes,FinancialTransactionTagType,BasicFinancialTransactionTagType>
    {
        public static FinancialTransactionTagTypes ForSet (FinancialTransactionTagSet set)
        {
            return FromArray(SwarmDb.GetDatabaseForReading().GetFinancialTransactionTagTypes(set));
        }

        public static FinancialTransactionTagTypes FromIdentities (int[] identities)
        {
            return FromArray(SwarmDb.GetDatabaseForReading().GetFinancialTransactionTagTypesByIdentity(identities));
        }

        public static FinancialTransactionTagTypes ForTransaction (FinancialTransaction transaction)
        {
            int[] tagTypeIdentities = SwarmDb.GetDatabaseForReading().GetFinancialTransactionTagTypes(transaction.Identity);

            if (tagTypeIdentities.Length == 0)
            {
                return new FinancialTransactionTagTypes(); // none, so return empty set
            }

            return FromIdentities(tagTypeIdentities);
        }
    }
}
