using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Support.LogEntries
{
    public class PasswordResetLogEntry: SecurityActionBase
    {
        public PasswordResetLogEntry()
        {
            // empty ctor for serialization
        }

        public PasswordResetLogEntry (Person concernedPerson, string remoteIPAddresses)
        {
            // This can only be done by the concerned person acting on ticket, so no acting person or org
            this.ConcernedPersonId = concernedPerson.Identity;
            this.RemoteIPAddresses = remoteIPAddresses;
        }

    }
}
