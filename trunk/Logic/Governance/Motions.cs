using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Logic.Support;
using Activizr.Basic.Types.Governance;
using Activizr.Database;

namespace Activizr.Logic.Governance
{
    public class Motions: PluralBase<Motions,Motion,BasicMotion>
    {
        static public Motions ForMeeting (Meeting meeting)
        {
            return Motions.FromArray(PirateDb.GetDatabase().GetMotions(meeting));
        }
    }
}