using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Financial;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Financial
{
    public class PaymentGroups : PluralBase<PaymentGroups, PaymentGroup, BasicPaymentGroup>
    {
        public static PaymentGroups ForOrganization (Organization organization)
        {
            return ForOrganization (organization, false);
        }

        public static PaymentGroups ForOrganization (Organization organization, bool includeClosed)
        {
            if (includeClosed)
            {
                return FromArray (SwarmDb.GetDatabaseForReading().GetPaymentGroups (organization));
            }
            return
                FromArray (SwarmDb.GetDatabaseForReading().GetPaymentGroups (organization, DatabaseCondition.OpenTrue));
        }
    }
}