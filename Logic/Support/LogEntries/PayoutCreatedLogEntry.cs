using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Support.LogEntries
{
    [Serializable]
    public class PayoutCreatedLogEntry: FinancialActionBase
    {
        public PayoutCreatedLogEntry()
        {
            // public ctor required for serializability
        }

        public PayoutCreatedLogEntry(Person payingPerson, Person beneficiaryPerson, Organization organization, Currency currency, double amount, string reason)
        {
            this.Amount = amount;
            this.Currency = currency.Code;
            this.OrganizationId = organization.Identity;
            this.DateTime = DateTime.UtcNow;
            this.Description = reason;
            this.ActingPersonId = payingPerson.Identity; // do not save name for data retention reasons
            this.BeneficiaryPersonId = (beneficiaryPerson != null ? beneficiaryPerson.Identity : 0);
        }

    }
}
