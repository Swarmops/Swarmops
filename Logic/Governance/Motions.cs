using Swarmops.Basic.Types.Governance;
using Swarmops.Database;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Governance
{
    public class Motions: PluralBase<Motions,Motion,BasicMotion>
    {
        static public Motions ForMeeting (Meeting meeting)
        {
            return Motions.FromArray(SwarmDb.GetDatabaseForReading().GetMotions(meeting));
        }
    }
}