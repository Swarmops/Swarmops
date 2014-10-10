using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;

namespace Swarmops.Logic.Governance
{
    public class MeetingElectionVote: BasicInternalPollVote
    {
        private MeetingElectionVote (BasicInternalPollVote basic): base (basic)
        {
            // empty private ctor            
        }

        static public MeetingElectionVote FromBasic (BasicInternalPollVote basic)
        {
            return new MeetingElectionVote(basic);
        }

        static public MeetingElectionVote FromIdentity (int internalPollVoteId)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetInternalPollVote(internalPollVoteId));
        }

        static public MeetingElectionVote FromVerificationCode (string verificationCode)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetInternalPollVote(verificationCode));
        }

        static public MeetingElectionVote Create (MeetingElection poll, Geography voteGeography)
        {
            return
                FromIdentity(SwarmDb.GetDatabaseForWriting().CreateInternalPollVote(poll.Identity, voteGeography.Identity,
                                                                           Authentication.
                                                                               CreateRandomPassword(12)));
        }

        public void AddDetail (int position, MeetingElectionCandidate candidate)
        {
            SwarmDb.GetDatabaseForWriting().CreateInternalPollVoteDetail(this.Identity, candidate.Identity, position);            
        }

        public void Clear()
        {
            SwarmDb.GetDatabaseForWriting().ClearInternalPollVote(this.Identity);
        }

        public int[] SelectedCandidateIdsInOrder
        {
            get { return SwarmDb.GetDatabaseForReading().GetInternalPollVoteDetails(this.Identity); }
        }

        /*
        public MeetingElectionCandidates Candidates
        {
            get { return MeetingElectionCandidates.FromIdentities(SelectedCandidateIdsInOrder); }
        }*/
    }
}