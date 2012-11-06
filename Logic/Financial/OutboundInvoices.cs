using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;
using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Financial
{
    public class OutboundInvoices: PluralBase<OutboundInvoices,OutboundInvoice,BasicOutboundInvoice>
    {
        public static OutboundInvoices ForOrganization (Organization organization)
        {
            return ForOrganization(organization, false);
        }

        public static OutboundInvoices ForOrganization (Organization organization, bool includeClosed)
        {
            if (includeClosed)
            {
                return FromArray(PirateDb.GetDatabaseForReading().GetOutboundInvoices(organization));
            }
            else
            {
                return FromArray (PirateDb.GetDatabaseForReading().GetOutboundInvoices(organization, DatabaseCondition.OpenTrue));
            }
        }
    }
}
