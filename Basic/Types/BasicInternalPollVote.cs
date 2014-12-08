using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    public class BasicInternalPollVote : IHasIdentity
    {
        public BasicInternalPollVote (int internalPollVoteId, int internalPollId, string verificationCode)
        {
            InternalPollVoteId = internalPollVoteId;
            InternalPollId = internalPollId;
            VerificationCode = verificationCode;
        }

        public BasicInternalPollVote (BasicInternalPollVote original)
            : this (original.InternalPollVoteId, original.InternalPollId, original.VerificationCode)
        {
            // empty copy ctor   
        }

        public int InternalPollVoteId { get; private set; }
        public int InternalPollId { get; private set; }
        public string VerificationCode { get; private set; }

        #region IHasIdentity Members

        public int Identity
        {
            get { return InternalPollVoteId; }
        }

        #endregion
    }
}