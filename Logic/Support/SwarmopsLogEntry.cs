using Swarmops.Basic.Types;
using Swarmops.Basic.Types.System;
using Swarmops.Common.Interfaces;
using Swarmops.Database;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Support
{
    public class SwarmopsLogEntry : BasicSwarmopsLogEntry
    {
        private SwarmopsLogEntry (BasicSwarmopsLogEntry basic) :
            base (basic)
        {
            // private ctor
        }

        public static SwarmopsLogEntry FromBasic (BasicSwarmopsLogEntry basic)
        {
            return new SwarmopsLogEntry (basic);
        }

        public static SwarmopsLogEntry FromIdentity (int swarmopsLogEntryId)
        {
            return FromBasic (SwarmDb.GetDatabaseForReading().GetSwarmopsLogEntry (swarmopsLogEntryId));
        }

        public static SwarmopsLogEntry FromIdentityAggressive (int swarmopsLogEntryId)
        {
            return FromBasic (SwarmDb.GetDatabaseForWriting().GetSwarmopsLogEntry (swarmopsLogEntryId));
            // Writing intentional
        }

        public static SwarmopsLogEntry Create(IXmlPayload logEntry, params object[] affectedObjects)
        {
            return Create(null, logEntry, affectedObjects);
        }

        public static SwarmopsLogEntry Create (Person person, IXmlPayload logEntry, params object[] affectedObjects)
        {
            SwarmopsLogEntry entry = SwarmopsLog.CreateEntry (person, logEntry);

            if (person != null)
            {
                entry.CreateAffectedObject (person);
            }

            foreach (IHasIdentity affectedObject in affectedObjects)
            {
                entry.CreateAffectedObject (affectedObject);
            }

            return entry;
        }

        public void CreateAffectedObject (IHasIdentity affectedObject)
        {
            string className = affectedObject.GetType().ToString();
            int dotIndex = className.LastIndexOf ('.');

            className = className.Substring (dotIndex + 1); // assumes there's always at least one dot in a class name


            SwarmDb.GetDatabaseForWriting()
                .CreateSwarmopsLogEntryAffectedObject (Identity, className, affectedObject.Identity);
        }
    }
}