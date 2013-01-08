using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Basic.Types.Governance;
using Swarmops.Database;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Governance
{
    public class MotionAmendments: PluralBase<MotionAmendments,MotionAmendment,BasicMotionAmendment>
    {
        static public MotionAmendments ForMotion (Motion motion)
        {
            return MotionAmendments.FromArray(PirateDb.GetDatabaseForReading().GetMotionAmendments(motion));
        }

        static public MotionAmendments ForMeeting (Meeting meeting)
        {
            return FromArray(PirateDb.GetDatabaseForReading().GetMotionAmendmentsForMeeting(meeting.Identity));
        }
    }
}