using System;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    public class BasicOutboundInvoiceItem : IHasIdentity
    {
        public BasicOutboundInvoiceItem(int outboundInvoiceItemId, int outboundInvoiceId, string description,
            Int64 amountCents)
        {
            OutboundInvoiceItemId = outboundInvoiceItemId;
            OutboundInvoiceId = outboundInvoiceId;
            Description = description;
            AmountCents = amountCents;
        }

        public BasicOutboundInvoiceItem(BasicOutboundInvoiceItem original) :
            this(original.OutboundInvoiceItemId, original.OutboundInvoiceId, original.Description, original.AmountCents)
        {
            // empty copy ctor
        }

        public int OutboundInvoiceItemId { get; private set; }
        public int OutboundInvoiceId { get; private set; }
        public string Description { get; private set; }
        public Int64 AmountCents { get; private set; }

        public int Identity
        {
            get { return OutboundInvoiceItemId; }
        }
    }
}