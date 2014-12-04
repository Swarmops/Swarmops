using System.Collections.Generic;
using Swarmops.Basic.Types;
using Swarmops.Database;

namespace Swarmops.Logic.Governance
{
    public class MeetingElectionVoters : List<MeetingElectionVoter>
    {
        public static MeetingElectionVoters FromArray(BasicInternalPollVoter[] basicArray)
        {
            MeetingElectionVoters result = new MeetingElectionVoters {Capacity = basicArray.Length*11/10};

            foreach (BasicInternalPollVoter basic in basicArray)
            {
                result.Add(MeetingElectionVoter.FromBasic(basic));
            }

            return result;
        }

        public static MeetingElectionVoters ForPoll(MeetingElection poll, bool includeClosed)
        {
            if (includeClosed)
            {
                return FromArray(SwarmDb.GetDatabaseForReading().GetInternalPollVoters(poll));
            }
            return FromArray(SwarmDb.GetDatabaseForReading().GetInternalPollVoters(poll, DatabaseCondition.OpenTrue));
        }

        public static MeetingElectionVoters ForPoll(MeetingElection poll)
        {
            return ForPoll(poll, false);
        }

        public static MeetingElectionVoters ForPollClosed(MeetingElection poll)
        {
            return FromArray(SwarmDb.GetDatabaseForReading().GetInternalPollVoters(poll, DatabaseCondition.OpenFalse));
        }
    }
}