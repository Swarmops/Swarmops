using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Logic.Support;
using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Financial
{
    public class OutboundInvoiceItems: PluralBase<OutboundInvoiceItems,OutboundInvoiceItem,BasicOutboundInvoiceItem>
    {
        public static OutboundInvoiceItems ForInvoice (OutboundInvoice invoice)
        {
            return FromArray(PirateDb.GetDatabaseForReading().GetOutboundInvoiceItems(invoice));
        }
    }
}
