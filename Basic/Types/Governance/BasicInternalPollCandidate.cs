using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types.Governance
{
    public class BasicInternalPollCandidate : IHasIdentity
    {
        public BasicInternalPollCandidate (int internalPollCandidateId, int internalPollId, int personId,
            string candidacyStatement)
        {
            this.InternalPollCandidateId = internalPollCandidateId;
            this.InternalPollId = internalPollId;
            this.PersonId = personId;
            this.CandidacyStatement = candidacyStatement;
        }

        public BasicInternalPollCandidate (BasicInternalPollCandidate original)
            : this (
                original.InternalPollCandidateId, original.InternalPollId, original.PersonId,
                original.CandidacyStatement)
        {
            // empty copy ctor
        }

        public int InternalPollCandidateId { get; private set; }
        public int InternalPollId { get; private set; }
        public int PersonId { get; private set; }
        public string CandidacyStatement { get; protected set; }

        #region IHasIdentity Members

        public int Identity
        {
            get { return this.InternalPollCandidateId; }
        }

        #endregion
    }
}