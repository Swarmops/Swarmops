using Swarmops.Logic.Structure;

namespace Swarmops.Logic.Swarm
{
    public class OfficerChain : People
    {
        private readonly int organizationId;

        private OfficerChain(People original, int organizationId)
        {
            InsertRange(0, original);
            this.organizationId = organizationId;
        }


        public new static OfficerChain FromOrganizationAndGeography(Organization org, Geography geo)
        {
            int[] concernedPeopleId = Roles.GetAllUpwardRoles(org.Identity, geo.Identity);
            People concernedPeople = FromIdentities(concernedPeopleId);

            return new OfficerChain(concernedPeople, org.Identity);
        }

        public void SendNotice(string subject, string body)
        {
            foreach (Person person in this)
            {
                person.SendOfficerNotice(subject, body, this.organizationId);
            }
        }
    }
}