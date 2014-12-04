using Swarmops.Basic.Types.Financial;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Financial
{
    public class FinancialTransactionTagSets :
        PluralBase<FinancialTransactionTagSets, FinancialTransactionTagSet, BasicFinancialTransactionTagSet>
    {
        public static FinancialTransactionTagSets ForOrganization(Organization organization)
        {
            return FromArray(SwarmDb.GetDatabaseForReading().GetFinancialTransactionTagSets(organization));
        }
    }
}