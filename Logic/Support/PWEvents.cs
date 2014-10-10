using System.Collections.Generic;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;
using Swarmops.Database;

namespace Swarmops.Logic.Support
{
    public class PWEvents
    {
        public static int CreateEvent (EventSource eventSource, EventType eventType, int actingPersonId,
                                       int organizationId,
                                       int geographyId, int affectedPersonId, int parameterInt, string parameterText)
        {
            //TODO: organizationId comes in hardcoded as 1 from a lot of places, should probably be changed based on affected person
            // to the party for that country if not swedish.

            return SwarmDb.GetDatabaseForWriting().CreateEvent(eventSource, eventType, actingPersonId, organizationId,
                                                       geographyId, affectedPersonId, parameterInt, parameterText);
        }

        public static BasicPWEvent[] GetTopUnprocessedEvents ()
        {
            return SwarmDb.GetDatabaseForReading().GetTopUnprocessedEvents();
        }

        public static BasicPWEvent[] ForPerson (int personId)
        {
            return SwarmDb.GetDatabaseForReading().GetEventsForPerson(personId);
        }

        public static Dictionary<int, List<BasicPWEvent>> ForPersons (int[] personIds)
        {
            return SwarmDb.GetDatabaseForReading().GetEventsForPersons(personIds, new EventType[] { });
        }

        public static Dictionary<int, List<BasicPWEvent>> ForPersons (int[] personIds, EventType[] eventTypes)
        {
            return SwarmDb.GetDatabaseForReading().GetEventsForPersons(personIds, eventTypes);
        }

        public static BasicPWEvent[] ByType (EventType eventType)
        {
            return SwarmDb.GetDatabaseForReading().GetEventsOfType(eventType);
        }

        public static void SetEventProcessed (int eventId)
        {
            SwarmDb.GetDatabaseForWriting().SetEventProcessed(eventId);
        }
    }
}