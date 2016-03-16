using System;
using System.IO;
using Swarmops.Database;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Support
{
    public class SwarmopsLog
    {
        public static SwarmopsLogEntry CreateEntry (Person person, IXmlPayload logEntry)
        {
            string logEntryClass = logEntry.GetType().ToString();

            if (logEntryClass.StartsWith ("Swarmops.Logic.Support.LogEntries."))
            {
                logEntryClass = logEntryClass.Substring ("Swarmops.Logic.Support.LogEntries.".Length);
            }

            int logEntryId = SwarmDb.GetDatabaseForWriting().CreateSwarmopsLogEntry (
                person != null ? person.Identity : 0, logEntryClass, logEntry.ToXml());

            return SwarmopsLogEntry.FromIdentityAggressive (logEntryId);
        }

        public static void DebugLog (string logEntry)
        {
            if (Path.DirectorySeparatorChar == '\\')
            {
                System.Diagnostics.Trace.WriteLine (logEntry);
            }
            else
            {
                // Linux - the reason we made this function to begin with

                File.AppendAllLines("/tmp/swarmops-debug.log", new string[] { DateTime.UtcNow.ToString("yyyy-MM-dd/HH:mm:ss.fff | ") + logEntry });
            }
        }
    }
}