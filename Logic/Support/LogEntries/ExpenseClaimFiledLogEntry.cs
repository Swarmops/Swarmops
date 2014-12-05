using System;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Support.LogEntries
{
    [Serializable]
    public class ExpenseClaimFiledLogEntry : FinancialActionBase
    {
        public ExpenseClaimFiledLogEntry()
        {
            // empty ctor needed for serialize
        }

        public ExpenseClaimFiledLogEntry (Person filingPerson, Person beneficiaryPerson, double amount,
            FinancialAccount budget, string reason)
        {
            Amount = amount;
            Currency = budget.Organization.Currency.Code;
            DateTime = DateTime.UtcNow;
            Description = reason;
            OrganizationId = budget.OrganizationId;
            FinancialAccountId = budget.Identity;
            FinancialAccountName = budget.Name; // redundancy in case of future name changes
            OwnerPersonId = budget.OwnerPersonId;
            OwnerPersonName = budget.OwnerPersonId != 0 ? budget.Owner.Name : string.Empty;
            ActingPersonId = filingPerson.Identity; // do not save name for data retention reasons
            BeneficiaryPersonId = beneficiaryPerson.Identity;
        }
    }
}