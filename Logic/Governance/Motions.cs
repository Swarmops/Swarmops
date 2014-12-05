using Swarmops.Basic.Types.Governance;
using Swarmops.Database;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Governance
{
    public class Motions : PluralBase<Motions, Motion, BasicMotion>
    {
        public static Motions ForMeeting (Meeting meeting)
        {
            return FromArray (SwarmDb.GetDatabaseForReading().GetMotions (meeting));
        }
    }
}