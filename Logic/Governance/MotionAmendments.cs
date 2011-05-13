using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Logic.Support;
using Activizr.Basic.Types.Governance;
using Activizr.Database;

namespace Activizr.Logic.Governance
{
    public class MotionAmendments: PluralBase<MotionAmendments,MotionAmendment,BasicMotionAmendment>
    {
        static public MotionAmendments ForMotion (Motion motion)
        {
            return MotionAmendments.FromArray(PirateDb.GetDatabase().GetMotionAmendments(motion));
        }

        static public MotionAmendments ForMeeting (Meeting meeting)
        {
            return FromArray(PirateDb.GetDatabase().GetMotionAmendmentsForMeeting(meeting.Identity));
        }
    }
}