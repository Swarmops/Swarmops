using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Swarmops.Basic.Interfaces;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Support
{
    public class SwarmopsLogEntry: BasicSwarmopsLogEntry
    {
        private SwarmopsLogEntry (BasicSwarmopsLogEntry basic):
            base (basic)
        {
            // private ctor
        }

        public static SwarmopsLogEntry FromBasic (BasicSwarmopsLogEntry basic)
        {
            return new SwarmopsLogEntry(basic);
        }

        public static SwarmopsLogEntry FromIdentity (int swarmopsLogEntryId)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetSwarmopsLogEntry(swarmopsLogEntryId));
        }

        public static SwarmopsLogEntry FromIdentityAggressive (int swarmopsLogEntryId)
        {
            return FromBasic(SwarmDb.GetDatabaseForWriting().GetSwarmopsLogEntry(swarmopsLogEntryId)); // Writing intentional
        }

        public static SwarmopsLogEntry Create (Person person, IXmlPayload logEntry)
        {
            return SwarmopsLog.CreateEntry(person, logEntry);
        }

        public void CreateAffectedObject (IHasIdentity affectedObject)
        {
            string className = affectedObject.GetType().ToString();
            int dotIndex = className.LastIndexOf('.');

            className = className.Substring(dotIndex); // assumes there's always at least one dot in a class name
            

            SwarmDb.GetDatabaseForWriting().CreateSwarmopsLogEntryAffectedObject(this.Identity, className, affectedObject.Identity);
        }
    }
}
