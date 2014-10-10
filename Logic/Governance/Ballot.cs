using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Swarm;

namespace Swarmops.Logic.Governance
{
    public class Ballot: BasicBallot
    {
        private Ballot (BasicBallot basic): base (basic)
        {
            // empty ctor
        }

        public static Ballot FromBasic (BasicBallot basic)
        {
            return new Ballot(basic);
        }

        public static Ballot FromIdentity (int ballotId)
        {
            return FromBasic(SwarmDb.GetDatabaseForReading().GetBallot(ballotId));
        }

        public static Ballot Create (Election election, Organization organization, Geography geography, string name, int ballotCount, string deliveryAddress)
        {
            return FromIdentity(SwarmDb.GetDatabaseForWriting().CreateBallot(election.Identity, name, organization.Identity, geography.Identity, ballotCount, deliveryAddress));
        }

        public void AddCandidate (Person person)
        {
            SwarmDb.GetDatabaseForWriting().CreateBallotCandidate(this.Identity, person.Identity);
        }

        public void ClearCandidates()
        {
            SwarmDb.GetDatabaseForWriting().ClearBallotCandidates(this.Identity);
        }

        public People Candidates
        {
            get
            {
                return People.FromIdentities(SwarmDb.GetDatabaseForReading().GetBallotCandidates(this.Identity), true);
            }
        }

        public Geography Geography
        {
            get { return Geography.FromIdentity(this.GeographyId); }
        }

        public new int Count
        {
            set 
            { 
                if (value != base.Count) {
                    base.Count = value;
                    SwarmDb.GetDatabaseForWriting().SetBallotCount(this.Identity, value);
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
                    SwarmDb.GetDatabaseForWriting().SetBallotDeliveryAddress(this.Identity, value);
                }
            }
            get { return base.DeliveryAddress; }
        }
    }
}
