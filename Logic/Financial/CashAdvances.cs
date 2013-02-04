using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swarmops.Basic.Types.Financial;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Financial
{
    public class CashAdvances: PluralBase<CashAdvances,CashAdvance,BasicCashAdvance>
    {
        static public CashAdvances ForOrganization(Organization organization)
        {
            return ForOrganization(organization, false);
        }

        static public CashAdvances ForOrganization(Organization organization, bool includeClosed)
        {
            return FromArray(PirateDb.GetDatabaseForReading().GetCashAdvances(organization,
                includeClosed ? DatabaseCondition.None : DatabaseCondition.OpenTrue));
        }

        public CashAdvances WhereUnattested
        {
            get
            {
                CashAdvances result = new CashAdvances();
                result.AddRange(this.Where(cashAdvance => cashAdvance.Attested == false));

                return result;
            }
        }

    }
}
