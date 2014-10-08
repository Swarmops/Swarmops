using System;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Support.LogEntries
{
    [Serializable]
    public class ExpenseClaimFiledLogEntry: FinancialActionBase
    {
        public ExpenseClaimFiledLogEntry()
        {
            // empty ctor needed for serialize
        }

        public ExpenseClaimFiledLogEntry(Person filingPerson, Person beneficiaryPerson, double amount, FinancialAccount budget, string reason)
        {
            this.Amount = amount;
            this.Currency = budget.Organization.Currency.Code;
            this.DateTime = DateTime.UtcNow;
            this.Description = reason;
            this.OrganizationId = budget.OrganizationId;
            this.FinancialAccountId = budget.Identity;
            this.FinancialAccountName = budget.Name; // redundancy in case of future name changes
            this.OwnerPersonId = budget.OwnerPersonId;
            this.OwnerPersonName = budget.OwnerPersonId != 0 ? budget.Owner.Name : string.Empty;
            this.ActingPersonId = filingPerson.Identity; // do not save name for data retention reasons
            this.BeneficiaryPersonId = beneficiaryPerson.Identity;
        }
    }
}
