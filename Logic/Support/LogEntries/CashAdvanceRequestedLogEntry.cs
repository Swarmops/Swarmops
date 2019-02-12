using System;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Support.LogEntries
{
    [Serializable]
    public class CashAdvanceRequestedLogEntry : FinancialActionBase
    {
        public CashAdvanceRequestedLogEntry()
        {
            // empty ctor needed for serialize
        }

        public CashAdvanceRequestedLogEntry (Person actingPerson, Person beneficiaryPerson, double amount,
            FinancialAccount budget, string reason)
        {
            Amount = amount;
            CurrencyCode = budget.Organization.Currency.Code;
            DateTime = DateTime.UtcNow;
            Description = reason;
            OrganizationId = budget.OrganizationId;
            FinancialAccountId = budget.Identity;
            FinancialAccountName = budget.Name; // redundancy in case of future name changes
            OwnerPersonId = budget.OwnerPersonId;
            OwnerPersonName = budget.OwnerPersonId != 0 ? budget.Owner.Name : string.Empty;
            ActingPersonId = actingPerson.Identity; // do not save name for data retention reasons
            BeneficiaryPersonId = beneficiaryPerson.Identity;
        }
    }
}