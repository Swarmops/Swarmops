using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Logic.Support;
using Swarmops.Logic.Swarm;
using Swarmops.Logic.Structure;
using System.Xml.Serialization;

namespace Swarmops.Logic.Communications.Resolution
{
    [Serializable]
    public class ParticipantResolver: PayloadBase<ParticipantResolver>, ICommsResolver, IHasGeography
    {
        [Obsolete("The parameterless ctor is provided for serializability and must not be called directly.", true)]
        public ParticipantResolver()
        {
            // Required for serialization. Never call directly.
        }

        public ParticipantResolver(Organization organization, Geography geography, bool includeSuborganizations = true)
        {
            this.IncludeSuborganizations = includeSuborganizations;
            this.GeographyId = geography.Identity;
            this.OrganizationId = organization.Identity;
        }

        public void Resolve(OutboundComm comm)
        {
            Organization organization = Organization.FromIdentity (OrganizationId);
            Geography geography = Geography.FromIdentity (GeographyId);

            People allParticipants = People.FromOrganizationAndGeography(organization, geography);

            foreach (Person person in allParticipants)
            {
                // Todo: Check if still participant? Check if declined correspondence?

                comm.AddRecipient(person);
            }
        }

        public bool IncludeSuborganizations { get; set; }
        public int OrganizationId { get; set; }
        public int GeographyId { get; set; }

        [XmlIgnore]
        public Geography Geography {  get { return Geography.FromIdentity(this.GeographyId); } }
    }
}
