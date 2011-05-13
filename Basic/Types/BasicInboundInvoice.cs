using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Basic.Interfaces;

namespace Activizr.Basic.Types
{
    public class BasicInboundInvoice: IHasIdentity
    {
        public BasicInboundInvoice (int inboundInvoiceId, int organizationId, DateTime createdDateTime,
            DateTime dueDate, Int64 amountCents, int budgetId, string supplier, string
            payToAccount, string ocr, string invoiceReference, bool attested, bool open,
            DateTime closedDateTime, int closedByPersonId)
        {
            this.InboundInvoiceId = inboundInvoiceId;
            this.OrganizationId = organizationId;
            this.CreatedDateTime = createdDateTime;
            this.DueDate = dueDate;
            this.AmountCents = amountCents;
            this.BudgetId = budgetId;
            this.Supplier = supplier;
            this.PayToAccount = payToAccount;
            this.Ocr = ocr;
            this.InvoiceReference = invoiceReference;
            this.Attested = attested;
            this.Open = open;
            this.ClosedDateTime = closedDateTime;
            this.ClosedByPersonId = closedByPersonId;
        }

        public BasicInboundInvoice (BasicInboundInvoice original)
            : this (original.InboundInvoiceId, original.OrganizationId, original.CreatedDateTime,
            original.DueDate, original.AmountCents, original.BudgetId, 
            original.Supplier, original.PayToAccount, original.Ocr, original.InvoiceReference,
            original.Attested, original.Open, original.ClosedDateTime, original.ClosedByPersonId)
        {
            // empty copy ctor
        }

        public int InboundInvoiceId { get; private set; }
        public DateTime CreatedDateTime { get; private set; }
        public DateTime DueDate { get; protected set; }
        public int BudgetId { get; protected set; }
        public bool Attested { get; protected set; }
        public bool Open { get; protected set; }
        public string PayToAccount { get; protected set; }
        public string Ocr { get; protected set; }
        public string InvoiceReference { get; protected set; }
        public DateTime ClosedDateTime { get; protected set; }
        public int ClosedByPersonId { get; protected set; }
        public int OrganizationId { get; protected set; }
        public string Supplier { get; protected set; }
        public Int64 AmountCents { get; protected set; }

        #region IHasIdentity Members

        public int Identity
        {
            get { return this.InboundInvoiceId; }
        }

        #endregion
    }
}
