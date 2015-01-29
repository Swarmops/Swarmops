using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Support.LogEntries
{
    [Serializable]
    public class PasswordResetRequest: SecurityActionBase
    {
        public PasswordResetRequest()
        {
            // empty ctor for serialization
        }

        public PasswordResetRequest (Person concernedPerson, string remoteIPAddresses, 
            Person actingPerson = null, Organization organization = null)
        {
            this.ConcernedPersonId = concernedPerson.Identity;
            this.RemoteIPAddresses = remoteIPAddresses;
            this.ActingPersonId = actingPerson != null ? actingPerson.Identity : 0;
            this.OrganizationId = organization != null ? organization.Identity : 0;
        }
    }
}
