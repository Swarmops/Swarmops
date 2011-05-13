using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Logic.Support;
using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Governance
{
    public class MeetingElectionVotes: PluralBase<MeetingElectionVotes,MeetingElectionVote,BasicInternalPollVote>
    {
        static public MeetingElectionVotes ForInternalPoll (MeetingElection poll)
        {
            return FromArray(PirateDb.GetDatabase().GetInternalPollVotes(poll));
        }
    }
}