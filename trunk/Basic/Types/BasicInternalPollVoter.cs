using System;
using System.Collections.Generic;
using System.Text;

namespace Swarmops.Basic.Types
{
    public class BasicInternalPollVoter
    {
        public BasicInternalPollVoter (int personId, int internalPollId, bool open, DateTime closedDateTime)
        {
            this.PersonId = personId;
            this.InternalPollId = internalPollId;
            this.Open = open;
            this.ClosedDateTime = closedDateTime;
        }

        public BasicInternalPollVoter (BasicInternalPollVoter original)
            : this (original.PersonId, original.InternalPollId, original.Open, original.ClosedDateTime)
        {
            // empty copy ctor
        }

        public int PersonId { get; private set; }
        public int InternalPollId { get; private set; }
        public bool Open { get; private set; }
        public DateTime ClosedDateTime { get; private set; }
    }


    public enum InternalPollVoterStatus
    {
        Unknown = 0,
        /// <summary>
        /// This voter is eligible and has not yet voted in this poll.
        /// </summary>
        CanVote,
        /// <summary>
        /// This voter is eligible to vote, but has already voted.
        /// </summary>
        HasAlreadyVoted,
        /// <summary>
        /// This person does is not on the list of eligible voters.
        /// </summary>
        NotEligibleForPoll
    }
}
