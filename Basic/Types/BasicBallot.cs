using Swarmops.Basic.Interfaces;

namespace Swarmops.Basic.Types
{
    public class BasicBallot : IHasIdentity
    {
        public BasicBallot (int ballotId, int electionId, int organizationId, int geographyId, string name, int count,
            string deliveryAddress)
        {
            BallotId = ballotId;
            ElectionId = electionId;
            OrganizationId = organizationId;
            GeographyId = geographyId;
            Name = name;
            Count = count;
            DeliveryAddress = deliveryAddress;
        }

        public BasicBallot (BasicBallot original)
            : this (
                original.BallotId, original.ElectionId, original.OrganizationId, original.GeographyId, original.Name,
                original.Count, original.DeliveryAddress)
        {
            // empty copy ctor
        }

        public int BallotId { get; private set; }
        public int ElectionId { get; private set; }
        public int OrganizationId { get; private set; }
        public int GeographyId { get; private set; }
        public string Name { get; private set; }
        public int Count { get; protected set; }
        public string DeliveryAddress { get; protected set; }

        public int Identity
        {
            get { return BallotId; }
        }
    }
}