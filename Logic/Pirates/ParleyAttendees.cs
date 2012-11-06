using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Logic.Support;
using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Pirates
{
    public class ParleyAttendees: PluralBase<ParleyAttendees,ParleyAttendee,BasicParleyAttendee>
    {
        public static ParleyAttendees ForParley (Parley parley)
        {
            return FromArray(PirateDb.GetDatabaseForReading().GetParleyAttendees(parley));
        }
    }
}
