using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Logic.Pirates;
using Activizr.Logic.Structure;
using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Governance
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
            return FromBasic(PirateDb.GetDatabase().GetBallot(ballotId));
        }

        public static Ballot Create (Election election, Organization organization, Geography geography, string name, int ballotCount, string deliveryAddress)
        {
            return FromIdentity(PirateDb.GetDatabase().CreateBallot(election.Identity, name, organization.Identity, geography.Identity, ballotCount, deliveryAddress));
        }

        public void AddCandidate (Person person)
        {
            PirateDb.GetDatabase().CreateBallotCandidate(this.Identity, person.Identity);
        }

        public void ClearCandidates()
        {
            PirateDb.GetDatabase().ClearBallotCandidates(this.Identity);
        }

        public People Candidates
        {
            get
            {
                return People.FromIdentities(PirateDb.GetDatabase().GetBallotCandidates(this.Identity), true);
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
                    PirateDb.GetDatabase().SetBallotCount(this.Identity, value);
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
                    PirateDb.GetDatabase().SetBallotDeliveryAddress(this.Identity, value);
                }
            }
            get { return base.DeliveryAddress; }
        }
    }
}
