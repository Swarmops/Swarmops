using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Basic.Interfaces;

namespace Activizr.Basic.Types
{
    public class BasicOutboundInvoiceItem: IHasIdentity
    {
        public BasicOutboundInvoiceItem (int outboundInvoiceItemId, int outboundInvoiceId, string description, Int64 amountCents)
        {
            this.OutboundInvoiceItemId = outboundInvoiceItemId;
            this.OutboundInvoiceId = outboundInvoiceId;
            this.Description = description;
            this.AmountCents = amountCents;
        }

        public BasicOutboundInvoiceItem(BasicOutboundInvoiceItem original): 
            this (original.OutboundInvoiceItemId, original.OutboundInvoiceId, original.Description, original.AmountCents)
        {
            // empty copy ctor
        }

        public int OutboundInvoiceItemId { get; private set; }
        public int OutboundInvoiceId { get; private set; }
        public string Description { get; private set; }
        public Int64 AmountCents { get; private set; }

        public int Identity
        {
            get { return this.OutboundInvoiceItemId; }
        }
    }
}
