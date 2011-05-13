using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Logic.Pirates;
using Activizr.Basic.Types;

namespace Activizr.Logic.Governance
{
    public class MeetingElectionVoter: BasicInternalPollVoter
    {
        private MeetingElectionVoter (BasicInternalPollVoter basic): base (basic)
        {
            // empty ctor
        }

        public static MeetingElectionVoter FromBasic (BasicInternalPollVoter basic)
        {
            return new MeetingElectionVoter(basic);
        }

        public Person Person
        {
            get
            {
                return Person.FromIdentity(base.PersonId);
            }
        }

        public MeetingElection Poll
        {
            get { return MeetingElection.FromIdentity(base.InternalPollId); }
        }

        public InternalPollVoterStatus VoterStatus
        {
            get
            {
                return base.Open ? InternalPollVoterStatus.CanVote : InternalPollVoterStatus.HasAlreadyVoted;
            }
        }
    }
}