using Swarmops.Logic.Structure;

namespace Swarmops.Logic.Swarm
{
    public class OfficerChain: People
    {
        private OfficerChain (People original, int organizationId)
        {
            this.InsertRange(0, original);
            this.organizationId = organizationId;
        }
        

        public static OfficerChain FromOrganizationAndGeography (Organization org, Geography geo)
        {
            int[] concernedPeopleId = Roles.GetAllUpwardRoles(org.Identity, geo.Identity);
            People concernedPeople = People.FromIdentities(concernedPeopleId);

            return new OfficerChain(concernedPeople, org.Identity);
        }

        public void SendNotice (string subject, string body)
        {
            foreach(Person person in this)
            {
                person.SendOfficerNotice(subject, body, this.organizationId);
            }
        }

        private readonly int organizationId;
    }
}
