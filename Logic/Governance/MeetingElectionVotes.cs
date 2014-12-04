using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Governance
{
    public class MeetingElectionVotes : PluralBase<MeetingElectionVotes, MeetingElectionVote, BasicInternalPollVote>
    {
        public static MeetingElectionVotes ForInternalPoll(MeetingElection poll)
        {
            return FromArray(SwarmDb.GetDatabaseForReading().GetInternalPollVotes(poll));
        }
    }
}