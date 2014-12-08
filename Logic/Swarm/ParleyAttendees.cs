using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Swarm
{
    public class ParleyAttendees : PluralBase<ParleyAttendees, ParleyAttendee, BasicParleyAttendee>
    {
        public static ParleyAttendees ForParley (Parley parley)
        {
            return FromArray (SwarmDb.GetDatabaseForReading().GetParleyAttendees (parley));
        }
    }
}