using System;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    public class BasicInboundInvoice : IHasIdentity
    {
        public BasicInboundInvoice(int inboundInvoiceId, int organizationId, DateTime createdDateTime,
            DateTime dueDate, Int64 amountCents, int budgetId, string supplier, string
                payToAccount, string ocr, string invoiceReference, bool attested, bool open,
            DateTime closedDateTime, int closedByPersonId)
        {
            InboundInvoiceId = inboundInvoiceId;
            OrganizationId = organizationId;
            CreatedDateTime = createdDateTime;
            DueDate = dueDate;
            AmountCents = amountCents;
            BudgetId = budgetId;
            Supplier = supplier;
            PayToAccount = payToAccount;
            Ocr = ocr;
            InvoiceReference = invoiceReference;
            Attested = attested;
            Open = open;
            ClosedDateTime = closedDateTime;
            ClosedByPersonId = closedByPersonId;
        }

        public BasicInboundInvoice(BasicInboundInvoice original)
            : this(original.InboundInvoiceId, original.OrganizationId, original.CreatedDateTime,
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
            get { return InboundInvoiceId; }
        }

        #endregion
    }
}