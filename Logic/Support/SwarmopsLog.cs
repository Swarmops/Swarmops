using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swarmops.Database;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Support
{
    public class SwarmopsLog
    {
        public static SwarmopsLogEntry CreateEntry (Person person, IXmlPayload logEntry)
        {
            string logEntryClass = logEntry.GetType().ToString();

            if (logEntryClass.StartsWith("Swarmops.Logic.Support.LogEntries."))
            {
                logEntryClass = logEntryClass.Substring("Swarmops.Logic.Support.LogEntries.".Length);
            }

            int logEntryId = SwarmDb.GetDatabaseForWriting().CreateSwarmopsLogEntry(
                person != null ? person.Identity : 0, logEntryClass, logEntry.ToXml());

            return SwarmopsLogEntry.FromIdentityAggressive(logEntryId);
        }
    }
}
