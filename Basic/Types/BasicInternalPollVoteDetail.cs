namespace Swarmops.Basic.Types
{
    public class BasicInternalPollVoteDetail
    {
        public BasicInternalPollVoteDetail (int internalPollVoteId, int internalPollCandidateId, int position)
        {
            InternalPollVoteId = internalPollVoteId;
            InternalPollCandidateId = internalPollCandidateId;
            Position = position;
        }

        public BasicInternalPollVoteDetail (BasicInternalPollVoteDetail original)
            : this (original.InternalPollVoteId, original.InternalPollCandidateId, original.Position)
        {
            // empty copy ctor
        }

        public int InternalPollVoteId { get; private set; }
        public int InternalPollCandidateId { get; private set; }
        public int Position { get; private set; }
    }
}