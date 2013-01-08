using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Basic.Types.Governance;
using Swarmops.Database;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Governance
{
    public class Motions: PluralBase<Motions,Motion,BasicMotion>
    {
        static public Motions ForMeeting (Meeting meeting)
        {
            return Motions.FromArray(PirateDb.GetDatabaseForReading().GetMotions(meeting));
        }
    }
}