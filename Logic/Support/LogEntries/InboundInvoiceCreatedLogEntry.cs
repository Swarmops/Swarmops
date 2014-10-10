using System;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Support.LogEntries
{
    [Serializable]
    public class InboundInvoiceCreatedLogEntry: FinancialActionBase
    {
        public InboundInvoiceCreatedLogEntry()
        {
            // empty ctor needed for serialize
        }

        public InboundInvoiceCreatedLogEntry(Person creatingPerson, string supplier, string description, double amount, FinancialAccount budget)
        {
            this.Amount = amount;
            this.Currency = budget.Organization.Currency.Code;
            this.DateTime = DateTime.UtcNow;
            this.Description = description;
            this.OrganizationId = budget.OrganizationId;
            this.FinancialAccountId = budget.Identity;
            this.FinancialAccountName = budget.Name; // redundancy in case of future name changes
            this.OwnerPersonId = budget.OwnerPersonId;
            this.OwnerPersonName = budget.OwnerPersonId != 0 ? budget.Owner.Name : string.Empty;
            this.ActingPersonId = creatingPerson.Identity; // do not save name for data retention reasons
            this.BeneficiaryPersonId = 0;
            this.Supplier = supplier;
        }

        public string Supplier { get; set; }
    }
}
