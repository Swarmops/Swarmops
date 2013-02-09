using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Financial
{
    public class OutboundInvoiceItems: PluralBase<OutboundInvoiceItems,OutboundInvoiceItem,BasicOutboundInvoiceItem>
    {
        public static OutboundInvoiceItems ForInvoice (OutboundInvoice invoice)
        {
            return FromArray(SwarmDb.GetDatabaseForReading().GetOutboundInvoiceItems(invoice));
        }
    }
}
