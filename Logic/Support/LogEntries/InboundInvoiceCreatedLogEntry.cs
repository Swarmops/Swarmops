using System;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Support.LogEntries
{
    [Serializable]
    public class InboundInvoiceCreatedLogEntry : FinancialActionBase
    {
        public InboundInvoiceCreatedLogEntry()
        {
            // empty ctor needed for serialize
        }

        public InboundInvoiceCreatedLogEntry (Person creatingPerson, string supplier, string description, double amount,
            FinancialAccount budget)
        {
            Amount = amount;
            CurrencyCode = budget.Organization.Currency.Code;
            DateTime = DateTime.UtcNow;
            Description = description;
            OrganizationId = budget.OrganizationId;
            FinancialAccountId = budget.Identity;
            FinancialAccountName = budget.Name; // redundancy in case of future name changes
            OwnerPersonId = budget.OwnerPersonId;
            OwnerPersonName = budget.OwnerPersonId != 0 ? budget.Owner.Name : string.Empty;
            ActingPersonId = creatingPerson.Identity; // do not save name for data retention reasons
            BeneficiaryPersonId = 0;
            Supplier = supplier;
        }

        public string Supplier { get; set; }
    }
}