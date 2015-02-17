using System;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types.Governance
{
    public class BasicInternalPoll : IHasIdentity
    {
        public BasicInternalPoll (int internalPollId, int createdByPersonId, int organizationId, int geographyId,
            string name, bool runningOpen, bool votingOpen, int maxVoteLength, DateTime runningOpens,
            DateTime runningCloses, DateTime votingOpens, DateTime votingCloses, InternalPollResultsType resultsType)
        {
            this.InternalPollId = internalPollId;
            this.CreatedByPersonId = createdByPersonId;
            this.OrganizationId = organizationId;
            this.GeographyId = geographyId;
            this.Name = name;
            this.RunningOpen = runningOpen;
            this.VotingOpen = votingOpen;
            this.MaxVoteLength = maxVoteLength;
            this.RunningOpens = runningOpens;
            this.RunningCloses = runningCloses;
            this.VotingOpens = votingOpens;
            this.VotingCloses = votingCloses;
            this.ResultsType = resultsType;
        }

        public BasicInternalPoll (BasicInternalPoll original)
            : this (
                original.InternalPollId, original.CreatedByPersonId, original.OrganizationId, original.GeographyId,
                original.Name, original.RunningOpen,
                original.VotingOpen, original.MaxVoteLength, original.RunningOpens, original.RunningCloses,
                original.VotingOpens, original.VotingCloses, original.ResultsType)
        {
            // empty copy ctor
        }

        public int MaxVoteLength { get; private set; }
        public int CreatedByPersonId { get; private set; }
        public int InternalPollId { get; private set; }
        public int OrganizationId { get; private set; }
        public int GeographyId { get; private set; }
        public string Name { get; protected set; }
        public bool RunningOpen { get; protected set; }
        public bool VotingOpen { get; protected set; }
        public DateTime RunningOpens { get; protected set; }
        public DateTime RunningCloses { get; protected set; }
        public DateTime VotingOpens { get; protected set; }
        public DateTime VotingCloses { get; protected set; }
        public InternalPollResultsType ResultsType { get; protected set; }

        #region IHasIdentity Members

        public int Identity
        {
            get { return this.InternalPollId; }
        }

        #endregion
    }


    public enum InternalPollResultsType
    {
        Unknown,

        /// <summary>
        ///     Primaries vote -- a Condorcet Schultze divided into six lists and some other twists
        /// </summary>
        Primaries,

        /// <summary>
        ///     A straight Condorcet Schultze, ranking the candidates
        /// </summary>
        Schulze,

        /// <summary>
        ///     Accept vote: Count the number of accepts for each candidate
        /// </summary>
        Accept
    }
}