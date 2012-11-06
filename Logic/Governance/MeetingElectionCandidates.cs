using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Logic.Support;
using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Governance
{
    public class MeetingElectionCandidates: PluralBase<MeetingElectionCandidates,MeetingElectionCandidate,BasicInternalPollCandidate>
    {
        public static MeetingElectionCandidates ForPoll (MeetingElection poll)
        {
            return FromArray(PirateDb.GetDatabaseForReading().GetInternalPollCandidates(poll));
        }
    }
}