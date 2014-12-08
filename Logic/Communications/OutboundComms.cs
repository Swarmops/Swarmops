using Swarmops.Basic.Types.Communications;
using Swarmops.Database;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Communications
{
    public class OutboundComms : PluralBase<OutboundComms, OutboundComm, BasicOutboundComm>
    {
        public static OutboundComms GetOpen()
        {
            return FromArray (SwarmDb.GetDatabaseForReading().GetOutboundComms (DatabaseCondition.OpenTrue));
        }
    }
}