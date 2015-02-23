using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Governance;
using Swarmops.Database;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Governance
{
    public class MeetingElectionCandidates :
        PluralBase<MeetingElectionCandidates, MeetingElectionCandidate, BasicInternalPollCandidate>
    {
        public static MeetingElectionCandidates ForPoll (MeetingElection poll)
        {
            return FromArray (SwarmDb.GetDatabaseForReading().GetInternalPollCandidates (poll));
        }
    }
}