using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Logic.Swarm;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Support.LogEntries
{
    [Serializable]
    public class PersonAddedLogEntry: ParticipationActionBase
    {
        [Obsolete("Do not call empty ctor directly. Intended for serialization only.", true)]
        public PersonAddedLogEntry()
        {
            // Do not call empty ctor directly. Intended for serialization only.
        }

        public PersonAddedLogEntry (Participation participation, Person actingPerson)
        {
            DateTime = System.DateTime.UtcNow;
            ParticipationId = participation.Identity;
            ActingPersonId = actingPerson.Identity;
            if (HttpContext.Current != null)
            {
                ActingIPAddress = SupportFunctions.GetMostLikelyRemoteIPAddress();
            }
        }
    }
}
