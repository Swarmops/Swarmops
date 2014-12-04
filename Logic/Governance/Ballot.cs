using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Governance
{
    public class Ballot : BasicBallot
    {
        private Ballot(BasicBallot basic) : base(basic)
        {
            // empty ctor
        }

        public People Candidates
        {
            get { return People.FromIdentities(SwarmDb.GetDatabaseForReading().GetBallotCandidates(Identity), true); }
        }

        public Geography Geography
        {
            get { return Geography.FromIdentity(GeographyId); }
        }

        public new int Count
        {
            set
            {
                if (value != base.Count)
                {
                    base.Count = value;
                    SwarmDb.GetDatabaseForWriting().SetBallotCount(Identity, value);
                }
            }
            get { return base.Count; }
        }

        public new string DeliveryAddress
        {
            set
            {
                if (value != base.DeliveryAddress)
                {
                    base.DeliveryAddress = value;
                    SwarmDb.GetDatabaseForWriting().SetBallotDeliveryAddress(Identity, value);
                }
            }
            get { return base.DeliveryAddress; }
        }

        public static Ballot FromBasic(BasicBallot basic)
        {
            return new Ballot(basic);
        }

        public static Ballot FromIdentity(int ballotId)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetBallot(ballotId));
        }

        public static Ballot Create(Election election, Organization organization, Geography geography, string name,
            int ballotCount, string deliveryAddress)
        {
            return
                FromIdentity(SwarmDb.GetDatabaseForWriting()
                    .CreateBallot(election.Identity, name, organization.Identity, geography.Identity, ballotCount,
                        deliveryAddress));
        }

        public void AddCandidate(Person person)
        {
            SwarmDb.GetDatabaseForWriting().CreateBallotCandidate(Identity, person.Identity);
        }

        public void ClearCandidates()
        {
            SwarmDb.GetDatabaseForWriting().ClearBallotCandidates(Identity);
        }
    }
}