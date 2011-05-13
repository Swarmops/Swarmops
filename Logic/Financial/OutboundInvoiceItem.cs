using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Financial
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
            return FromBasic(PirateDb.GetDatabase().GetOutboundInvoiceItem(outboundInvoiceItemId));
        }

        public decimal Amount
        {
            get { return (decimal) AmountCents/100.0m; }
        }
    }
}
