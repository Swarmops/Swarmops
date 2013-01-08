using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Basic.Types;
using Swarmops.Database;

namespace Swarmops.Logic.Financial
{
    public class OutboundInvoiceItem: BasicOutboundInvoiceItem
    {
        private OutboundInvoiceItem (BasicOutboundInvoiceItem basic): base (basic)
        {
            // empty ctor
        }

        public static OutboundInvoiceItem FromBasic (BasicOutboundInvoiceItem basic)
        {
            return new OutboundInvoiceItem(basic);
        }

        public static OutboundInvoiceItem FromIdentity (int outboundInvoiceItemId)
        {
            return FromBasic(PirateDb.GetDatabaseForReading().GetOutboundInvoiceItem(outboundInvoiceItemId));
        }

        public decimal Amount
        {
            get { return (decimal) AmountCents/100.0m; }
        }
    }
}
