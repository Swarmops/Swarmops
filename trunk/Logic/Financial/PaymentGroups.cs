using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;
using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Financial
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
                return FromArray(PirateDb.GetDatabase().GetPaymentGroups(organization));
            }
            else
            {
                return FromArray(PirateDb.GetDatabase().GetPaymentGroups(organization, DatabaseCondition.OpenTrue));
            }
        }
    }
}
