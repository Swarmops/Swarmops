using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Basic.Interfaces;

namespace Activizr.Basic.Types
{
    public class BasicPaymentGroup: IHasIdentity
    {
        public BasicPaymentGroup (int paymentGroupId, int organizationId, DateTime dateTime, int currencyId, Int64 amountCents, string tag, int createdByPersonId, DateTime createdDateTime, bool open)
        {
            this.PaymentGroupId = paymentGroupId;
            this.OrganizationId = organizationId;
            this.Tag = tag;
            this.DateTime = dateTime;
            this.AmountCents = amountCents;
            this.CurrencyId = currencyId;
            this.CreatedByPersonId = createdByPersonId;
            this.CreatedDateTime = createdDateTime;
            this.Open = open;
        }

        public BasicPaymentGroup (BasicPaymentGroup original)
            : this(original.PaymentGroupId, original.OrganizationId, original.DateTime, original.CurrencyId, original.AmountCents, original.Tag, original.CreatedByPersonId, original.CreatedDateTime, original.Open)
        {
            // empty copy ctor
        }

        public int PaymentGroupId { get; private set; }
        public int OrganizationId { get; private set; }
        public string Tag { get; protected set; }
        public DateTime DateTime { get; private set; }
        public Int64 AmountCents { get; protected set; }
        public int CurrencyId { get; private set; }
        public int CreatedByPersonId { get; private set; }
        public DateTime CreatedDateTime { get; private set; }
        public bool Open { get; protected set; }

        public int Identity
        {
            get { return this.PaymentGroupId; }
        }
    }
}
