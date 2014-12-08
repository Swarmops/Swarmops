using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Support.LogEntries
{
    [Serializable]
    public class PersonAddedLogEntry: MembershipActionBase
    {
        [Obsolete("Do not call empty ctor directly. Intended for serialization only.", true)]
        public PersonAddedLogEntry()
        {
            // Do not call empty ctor directly. Intended for serialization only.
        }

        public PersonAddedLogEntry (Membership membership, Person actingPerson)
        {
            DateTime = System.DateTime.UtcNow;
            MembershipId = membership.Identity;
            ActingPersonId = actingPerson.Identity;
        }
    }
}
