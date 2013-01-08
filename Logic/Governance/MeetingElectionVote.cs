using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Logic.Security;
using Swarmops.Logic.Structure;
using Swarmops.Basic.Types;
using Swarmops.Database;

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
            return FromBasic(PirateDb.GetDatabaseForReading().GetInternalPollVote(internalPollVoteId));
        }

        static public MeetingElectionVote FromVerificationCode (string verificationCode)
        {
            return FromBasic(PirateDb.GetDatabaseForReading().GetInternalPollVote(verificationCode));
        }

        static public MeetingElectionVote Create (MeetingElection poll, Geography voteGeography)
        {
            return
                FromIdentity(PirateDb.GetDatabaseForWriting().CreateInternalPollVote(poll.Identity, voteGeography.Identity,
                                                                           Authentication.
                                                                               CreateRandomPassword(12)));
        }

        public void AddDetail (int position, MeetingElectionCandidate candidate)
        {
            PirateDb.GetDatabaseForWriting().CreateInternalPollVoteDetail(this.Identity, candidate.Identity, position);            
        }

        public void Clear()
        {
            PirateDb.GetDatabaseForWriting().ClearInternalPollVote(this.Identity);
        }

        public int[] SelectedCandidateIdsInOrder
        {
            get { return PirateDb.GetDatabaseForReading().GetInternalPollVoteDetails(this.Identity); }
        }

        /*
        public MeetingElectionCandidates Candidates
        {
            get { return MeetingElectionCandidates.FromIdentities(SelectedCandidateIdsInOrder); }
        }*/
    }
}