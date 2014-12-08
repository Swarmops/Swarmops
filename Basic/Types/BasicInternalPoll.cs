using System;
using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    public class BasicInternalPoll : IHasIdentity
    {
        public BasicInternalPoll (int internalPollId, int createdByPersonId, int organizationId, int geographyId,
            string name, bool runningOpen, bool votingOpen, int maxVoteLength, DateTime runningOpens,
            DateTime runningCloses, DateTime votingOpens, DateTime votingCloses, InternalPollResultsType resultsType)
        {
            InternalPollId = internalPollId;
            CreatedByPersonId = createdByPersonId;
            OrganizationId = organizationId;
            GeographyId = geographyId;
            Name = name;
            RunningOpen = runningOpen;
            VotingOpen = votingOpen;
            MaxVoteLength = maxVoteLength;
            RunningOpens = runningOpens;
            RunningCloses = runningCloses;
            VotingOpens = votingOpens;
            VotingCloses = votingCloses;
            ResultsType = resultsType;
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
            get { return InternalPollId; }
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