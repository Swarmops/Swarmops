using System;
using Swarmops.Common.Interfaces;

namespace Swarmops.Basic.Types.Financial
{
    public class BasicRefund : IHasIdentity
    {
        public BasicRefund (int refundId, int paymentId, bool open, Int64 amountCents, int createdByPersonId,
            DateTime createdDateTime, DateTime closedDateTime)
        {
            this.RefundId = refundId;
            this.PaymentId = paymentId;
            this.Open = open;
            this.AmountCents = amountCents;
            this.CreatedByPersonId = createdByPersonId;
            this.CreatedDateTime = createdDateTime;
            this.ClosedDateTime = closedDateTime;
        }

        public BasicRefund (BasicRefund original)
            : this (
                original.RefundId, original.PaymentId, original.Open, original.AmountCents, original.CreatedByPersonId,
                original.CreatedDateTime, original.ClosedDateTime)
        {
            // Empty copy ctor
        }

        public int RefundId { get; private set; }
        public int PaymentId { get; private set; }
        public bool Open { get; protected set; }
        public Int64 AmountCents { get; private set; }
        public int CreatedByPersonId { get; private set; }
        public DateTime CreatedDateTime { get; private set; }
        public DateTime ClosedDateTime { get; protected set; }

        public int Identity
        {
            get { return this.RefundId; }
        }
    }
}