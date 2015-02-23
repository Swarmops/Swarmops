using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Governance;
using Swarmops.Database;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;

namespace Swarmops.Logic.Governance
{
    public class MeetingElectionVote : BasicInternalPollVote
    {
        private MeetingElectionVote (BasicInternalPollVote basic) : base (basic)
        {
            // empty private ctor            
        }

        public int[] SelectedCandidateIdsInOrder
        {
            get { return SwarmDb.GetDatabaseForReading().GetInternalPollVoteDetails (Identity); }
        }

        public static MeetingElectionVote FromBasic (BasicInternalPollVote basic)
        {
            return new MeetingElectionVote (basic);
        }

        public static MeetingElectionVote FromIdentity (int internalPollVoteId)
        {
            return FromBasic (SwarmDb.GetDatabaseForReading().GetInternalPollVote (internalPollVoteId));
        }

        public static MeetingElectionVote FromVerificationCode (string verificationCode)
        {
            return FromBasic (SwarmDb.GetDatabaseForReading().GetInternalPollVote (verificationCode));
        }

        public static MeetingElectionVote Create (MeetingElection poll, Geography voteGeography)
        {
            return
                FromIdentity (SwarmDb.GetDatabaseForWriting()
                    .CreateInternalPollVote (poll.Identity, voteGeography.Identity,
                        Authentication.
                            CreateRandomPassword (12)));
        }

        public void AddDetail (int position, MeetingElectionCandidate candidate)
        {
            SwarmDb.GetDatabaseForWriting().CreateInternalPollVoteDetail (Identity, candidate.Identity, position);
        }

        public void Clear()
        {
            SwarmDb.GetDatabaseForWriting().ClearInternalPollVote (Identity);
        }

        /*
        public MeetingElectionCandidates Candidates
        {
            get { return MeetingElectionCandidates.FromIdentities(SelectedCandidateIdsInOrder); }
        }*/
    }
}