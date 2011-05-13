using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Logic.Governance;

namespace Activizr.Utility.BotCode
{
    public class InternalPollRandomizer
    {
        // Randomizes the order of candidates on the open polls.
        static public void Run()
        {

            // TODO: Foreach open poll...

            MeetingElectionCandidates candidates = MeetingElectionCandidates.ForPoll(MeetingElection.Primaries2010);

            foreach (MeetingElectionCandidate candidate in candidates)
            {
                candidate.RandomizeSortOrder();
            }
        }
    }
}
