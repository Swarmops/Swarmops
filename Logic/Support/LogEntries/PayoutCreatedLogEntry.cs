using System;
using Swarmops.Logic.Financial;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Support.LogEntries
{
    [Serializable]
    public class PayoutCreatedLogEntry : FinancialActionBase
    {
        public PayoutCreatedLogEntry()
        {
            // public ctor required for serializability
        }

        public PayoutCreatedLogEntry (Person payingPerson, Person beneficiaryPerson, Organization organization,
            Currency currency, double amount, string reason)
        {
            Amount = amount;
            Currency = currency.Code;
            OrganizationId = organization.Identity;
            DateTime = DateTime.UtcNow;
            Description = reason;
            ActingPersonId = payingPerson.Identity; // do not save name for data retention reasons
            BeneficiaryPersonId = (beneficiaryPerson != null ? beneficiaryPerson.Identity : 0);
        }
    }
}