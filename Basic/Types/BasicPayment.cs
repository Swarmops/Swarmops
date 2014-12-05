using System;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    public class BasicPayment : IHasIdentity
    {
        public BasicPayment (int paymentId, int paymentGroupId, Int64 amountCents, string reference, string fromAccount,
            string key, bool hasImage, int outboundInvoiceId)
        {
            PaymentId = paymentId;
            PaymentGroupId = paymentGroupId;
            AmountCents = amountCents;
            Reference = reference;
            FromAccount = fromAccount;
            Key = key;
            HasImage = hasImage;
            OutboundInvoiceId = outboundInvoiceId;
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
            get { return PaymentId; }
        }
    }
}