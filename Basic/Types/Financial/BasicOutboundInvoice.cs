using System;
using Swarmops.Common.Interfaces;

namespace Swarmops.Basic.Types.Financial
{
    public class BasicOutboundInvoice : IHasIdentity
    {
        public BasicOutboundInvoice (int outboundInvoiceId, string customerName, string invoiceAddressPaper,
            string invoiceAddressMail, int currencyId, int organizationId, int budgetId,
            DateTime createdDateTime, int createdByPersonId, DateTime dueDate, int reminderCount, string reference,
            bool domestic, bool open, bool sent, string securityCode, string theirReference)
        {
            this.OutboundInvoiceId = outboundInvoiceId;
            this.CustomerName = customerName;
            this.InvoiceAddressPaper = invoiceAddressPaper;
            this.InvoiceAddressMail = invoiceAddressMail;
            this.CurrencyId = currencyId;
            this.OrganizationId = organizationId;
            this.BudgetId = budgetId;
            this.CreatedDateTime = createdDateTime;
            this.CreatedByPersonId = createdByPersonId;
            this.DueDate = dueDate;
            this.ReminderCount = reminderCount;
            this.Reference = reference;
            this.Domestic = domestic;
            this.Open = open;
            this.Sent = sent;
            this.SecurityCode = securityCode;
            this.TheirReference = theirReference;
        }


        public BasicOutboundInvoice (BasicOutboundInvoice original) :
            this (
            original.OutboundInvoiceId, original.CustomerName, original.InvoiceAddressPaper, original.InvoiceAddressMail,
            original.CurrencyId, original.OrganizationId, original.BudgetId, original.CreatedDateTime,
            original.CreatedByPersonId, original.DueDate, original.ReminderCount, original.Reference, original.Domestic,
            original.Open, original.Sent, original.SecurityCode, original.TheirReference)
        {
            // empty copy ctor
        }

        public int OutboundInvoiceId { get; private set; }
        public string CustomerName { get; private set; }
        public string InvoiceAddressPaper { get; private set; }
        public string InvoiceAddressMail { get; private set; }
        public int CurrencyId { get; private set; }
        public int OrganizationId { get; private set; }
        public int BudgetId { get; protected set; }
        public DateTime CreatedDateTime { get; private set; }
        public int CreatedByPersonId { get; private set; }
        public DateTime DueDate { get; private set; }
        public int ReminderCount { get; protected set; }
        public string Reference { get; protected set; }
        public bool Domestic { get; private set; }
        public bool Open { get; protected set; }
        public bool Sent { get; protected set; }
        public string SecurityCode { get; private set; }
        public string TheirReference { get; protected set; }


        public int Identity
        {
            get { return this.OutboundInvoiceId; }
        }
    }
}