using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Governance;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Governance
{
    public class MeetingElectionVoter : BasicInternalPollVoter
    {
        private MeetingElectionVoter (BasicInternalPollVoter basic) : base (basic)
        {
            // empty ctor
        }

        public Person Person
        {
            get { return Person.FromIdentity (base.PersonId); }
        }

        public MeetingElection Poll
        {
            get { return MeetingElection.FromIdentity (base.InternalPollId); }
        }

        public InternalPollVoterStatus VoterStatus
        {
            get { return base.Open ? InternalPollVoterStatus.CanVote : InternalPollVoterStatus.HasAlreadyVoted; }
        }

        public static MeetingElectionVoter FromBasic (BasicInternalPollVoter basic)
        {
            return new MeetingElectionVoter (basic);
        }
    }
}