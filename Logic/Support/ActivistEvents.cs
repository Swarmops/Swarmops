using Swarmops.Common.Enums;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Support
{
    public class ActivistEvents
    {
        public static void TerminateActivistWithLogging (Person p, EventSource eventSourceSignupPage)
        {
            int orgId = 1;
            PWEvents.CreateEvent (eventSourceSignupPage,
                EventType.LostActivist,
                p.Identity,
                orgId,
                p.Geography.Identity,
                p.Identity,
                0,
                string.Empty);
            p.TerminateActivist();
            PWLog.Write (PWLogItem.Person, p.Identity, PWLogAction.ActivistLost, "Lost as activist", string.Empty);
        }


        public static void CreateActivistWithLogging (Geography geo, Person newActivist, string logMessage,
            EventSource evtSrc, bool isPublic, bool isConfirmed, int orgId)
        {
            PWEvents.CreateEvent (evtSrc,
                EventType.NewActivist,
                newActivist.Identity,
                orgId,
                geo.Identity,
                newActivist.Identity,
                0,
                string.Empty);
            newActivist.CreateActivist (isPublic, isConfirmed);
            PWLog.Write (newActivist, PWLogItem.Person, newActivist.Identity, PWLogAction.ActivistJoin,
                "New activist joined.", logMessage);
        }
    }
}