using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Swarm
{
    public class ParleyAttendees: PluralBase<ParleyAttendees,ParleyAttendee,BasicParleyAttendee>
    {
        public static ParleyAttendees ForParley (Parley parley)
        {
            return FromArray(PirateDb.GetDatabaseForReading().GetParleyAttendees(parley));
        }
    }
}
