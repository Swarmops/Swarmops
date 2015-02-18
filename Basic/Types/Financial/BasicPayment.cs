using System;
using Swarmops.Common.Interfaces;

namespace Swarmops.Basic.Types.Financial
{
    public class BasicPayment : IHasIdentity
    {
        public BasicPayment (int paymentId, int paymentGroupId, Int64 amountCents, string reference, string fromAccount,
            string key, bool hasImage, int outboundInvoiceId)
        {
            this.PaymentId = paymentId;
            this.PaymentGroupId = paymentGroupId;
            this.AmountCents = amountCents;
            this.Reference = reference;
            this.FromAccount = fromAccount;
            this.Key = key;
            this.HasImage = hasImage;
            this.OutboundInvoiceId = outboundInvoiceId;
        }

        public BasicPayment (BasicPayment original)
            : this (
                original.PaymentId, original.PaymentGroupId, original.AmountCents, original.Reference,
                original.FromAccount, original.Key, original.HasImage, original.OutboundInvoiceId)
        {
            // empty copy ctor
        }

        public int PaymentId { get; private set; }
        public int PaymentGroupId { get; private set; }
        public Int64 AmountCents { get; private set; }
        public string Reference { get; private set; }
        public string FromAccount { get; private set; }
        public string Key { get; private set; } // may need to expand with URL or similar later
        public bool HasImage { get; private set; }
        public int OutboundInvoiceId { get; protected set; }

        public int Identity
        {
            get { return this.PaymentId; }
        }
    }
}