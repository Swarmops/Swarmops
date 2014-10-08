using System;
using System.Collections.Generic;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Governance
{
    public class MeetingElection: BasicInternalPoll
    {
        private MeetingElection(BasicInternalPoll basic) : base(basic)
        {
            // private ctor
        }

        public static MeetingElection FromBasic (BasicInternalPoll basic)
        {
            return new MeetingElection(basic);
        }

        public static MeetingElection Create (Person creator, Organization org, Geography geo, string name, InternalPollResultsType resultsType, int maxVoteLength, DateTime runningOpens, DateTime runningCloses, DateTime votingOpens, DateTime votingCloses)
        {
            return
                FromIdentity(SwarmDb.GetDatabaseForWriting().CreateInternalPoll(org.Identity, geo.Identity, name, maxVoteLength,
                                                                       resultsType, creator.Identity, runningOpens,
                                                                       runningCloses, votingOpens, votingCloses));
        }


        public static MeetingElection FromIdentity (int internalPollId)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetInternalPoll(internalPollId));
        }

        
        public static MeetingElection Primaries2010Simulation
        {
            get { return FromIdentity(1); }
        }

        public static MeetingElection Primaries2010
        {
            get { return FromIdentity(2); }
        }

        public static MeetingElection UpDelegates2010
        {
            get { return FromIdentity(3); }
        }

        public MeetingElectionCandidates Candidates
        {
            get { return MeetingElectionCandidates.ForPoll(this); }
        }

        public void AddCandidate (Person person, string candidacyStatement)
        {
            MeetingElectionCandidate.Create(this, person, candidacyStatement);
        }

        public void AddVoter (Person person)
        {
            SwarmDb.GetDatabaseForWriting().CreateInternalPollVoter(this.Identity, person.Identity);
        }

        public MeetingElectionVote CreateVote(Person person, string ipAddress)
        {
            if (SwarmDb.GetDatabaseForReading().GetInternalPollVoterStatus(this.Identity, person.Identity) != InternalPollVoterStatus.CanVote)
            {
                throw new InvalidOperationException("Voter status is not open");
            }

            Geography voteGeography = person.Geography;

            // Make sure that the vote geography is not at a lower level than ElectoralCircuit.

            while (voteGeography.ParentGeographyId != 0 && !voteGeography.AtLevel(GeographyLevel.ElectoralCircuit))
            {
                voteGeography = voteGeography.Parent;
            }

            if (voteGeography.ParentGeographyId == 0)
            {
                voteGeography = person.Geography;
            }

            SwarmDb.GetDatabaseForWriting().CloseInternalPollVoter(this.Identity, person.Identity, ipAddress);
            return MeetingElectionVote.Create(this, voteGeography);
        }

        public InternalPollVoterStatus GetVoterStatus (Person person)
        {
            return SwarmDb.GetDatabaseForReading().GetInternalPollVoterStatus(this.Identity, person.Identity);
        }

        public Dictionary<int,int> GetCandidatePersonMap()
        {
            return SwarmDb.GetDatabaseForReading().GetCandidateIdPersonIdMap(this.Identity);
        }

        public MeetingElectionVoters GetClosedVoters()
        {
            return MeetingElectionVoters.ForPollClosed(this);
        }

        public Organization Organization
        {
            get { return Structure.Organization.FromIdentity(base.OrganizationId); }
        }

        public new bool VotingOpen
        {
            get { return base.VotingOpen; }
            set
            {
                if (value != base.VotingOpen)
                {
                    SwarmDb.GetDatabaseForWriting().SetInternalPollVotingOpen(this.Identity, value);
                    base.VotingOpen = value;
                }
            }
        }

        public new bool RunningOpen
        {
            get { return base.RunningOpen; }
            set
            {
                if (value != base.RunningOpen)
                {
                    SwarmDb.GetDatabaseForWriting().SetInternalPollRunningOpen(this.Identity, value);
                    base.RunningOpen = value;
                }
            }
        }

        public Person Creator
        {
            get { return Person.FromIdentity(base.CreatedByPersonId); }
        }
    }
}