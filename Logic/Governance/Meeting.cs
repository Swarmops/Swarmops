using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Basic.Types.Governance;
using Activizr.Database;

namespace Activizr.Logic.Governance
{
    public class Meeting: BasicMeeting
    {
        private Meeting (BasicMeeting basic): base (basic)
        {
            // empty ctor
        }

        public static Meeting FromBasic (BasicMeeting basicMeeting)
        {
            return new Meeting(basicMeeting);
        }

        public static Meeting FromIdentity (int meetingId)
        {
            return FromBasic(PirateDb.GetDatabaseForReading().GetMeeting(meetingId));
        }
    }
}