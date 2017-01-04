using Swarmops.Basic.Types.Communications;
using Swarmops.Database;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Communications
{
    public class OutboundCommRecipients :
        PluralBase<OutboundCommRecipients, OutboundCommRecipient, BasicOutboundCommRecipient>
    {
        public static OutboundCommRecipients ForOutboundComm (OutboundComm comm)
        {
            return FromArray (SwarmDb.GetDatabaseForReading().GetOutboundCommRecipients (comm));
        }

        public static OutboundCommRecipients ForOutboundCommLimited (OutboundComm comm, int limit)
        {
            return FromArray(SwarmDb.GetDatabaseForReading().GetOutboundCommRecipients(comm, new RecordLimit(limit)));
        }
    }
}