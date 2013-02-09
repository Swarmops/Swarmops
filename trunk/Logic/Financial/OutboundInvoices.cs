using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Financial
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
                return FromArray(SwarmDb.GetDatabaseForReading().GetOutboundInvoices(organization));
            }
            else
            {
                return FromArray (SwarmDb.GetDatabaseForReading().GetOutboundInvoices(organization, DatabaseCondition.OpenTrue));
            }
        }
    }
}
