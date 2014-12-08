using Swarmops.Basic.Types.Governance;
using Swarmops.Database;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Governance
{
    public class MotionAmendments : PluralBase<MotionAmendments, MotionAmendment, BasicMotionAmendment>
    {
        public static MotionAmendments ForMotion (Motion motion)
        {
            return FromArray (SwarmDb.GetDatabaseForReading().GetMotionAmendments (motion));
        }

        public static MotionAmendments ForMeeting (Meeting meeting)
        {
            return FromArray (SwarmDb.GetDatabaseForReading().GetMotionAmendmentsForMeeting (meeting.Identity));
        }
    }
}