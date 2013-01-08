using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Financial
{
    public class PaymentGroups: PluralBase<PaymentGroups,PaymentGroup,BasicPaymentGroup>
    {
        public static PaymentGroups ForOrganization (Organization organization)
        {
            return ForOrganization(organization, false);
        }

        public static PaymentGroups ForOrganization (Organization organization, bool includeClosed)
        {
            if (includeClosed)
            {
                return FromArray(PirateDb.GetDatabaseForReading().GetPaymentGroups(organization));
            }
            else
            {
                return FromArray(PirateDb.GetDatabaseForReading().GetPaymentGroups(organization, DatabaseCondition.OpenTrue));
            }
        }
    }
}
