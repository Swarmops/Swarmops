using System;
using System.Collections.Generic;
using Swarmops.Basic.Types.Financial;
using Swarmops.Database;
using Swarmops.Logic.Structure;

namespace Swarmops.Logic.Financial
{
    public class FinancialAccountRows : List<FinancialAccountRow>
    {
        public static FinancialAccountRows FromArray (BasicFinancialAccountRow[] array)
        {
            FinancialAccountRows result = new FinancialAccountRows {Capacity = (array.Length*11/10)};

            foreach (BasicFinancialAccountRow basic in array)
            {
                result.Add (FinancialAccountRow.FromBasic (basic));
            }

            return result;
        }

        public static FinancialAccountRows ForOrganization(Organization organization, DateTime fromDate,
            DateTime toDate)
        {
            FinancialAccounts orgAccounts = FinancialAccounts.ForOrganization (organization);
            orgAccounts.GetRows (fromDate, toDate);
            return
                FromArray (SwarmDb.GetDatabaseForReading()
                    .GetFinancialAccountRows (orgAccounts.Identities, fromDate, toDate, false));
        }
    }
}