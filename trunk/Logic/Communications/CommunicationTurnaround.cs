using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Swarm;
using Swarmops.Logic.Structure;

namespace Swarmops.Logic.Communications
{
    public class CommunicationTurnaround: BasicCommunicationTurnaround
    {
        private CommunicationTurnaround(BasicCommunicationTurnaround basic): base (basic)
        {
            // private ctor
        }

        public static CommunicationTurnaround FromBasic (BasicCommunicationTurnaround basic)
        {
            return new CommunicationTurnaround(basic);
        }

        public static CommunicationTurnaround FromIdentity (Organization organization, int communicationTypeId, int communicationId)
        {
            return
                FromBasic(SwarmDb.GetDatabaseForReading().GetCommunicationTurnaround(organization.Identity, communicationTypeId,
                                                                            communicationId));
        }

        public static CommunicationTurnaround Create (Organization organization, int communicationId, DateTime dateTimeOpened)
        {
            SwarmDb.GetDatabaseForWriting().CreateCommunicationTurnaround(organization.Identity, 1, communicationId, dateTimeOpened);
            return FromIdentity(organization, 1, communicationId);
        }

        public void SetResponded (DateTime dateTime, Person person)
        {
            if (!Open)
            {
                return;
            }

            if (Responded)
            {
                return;
            }

            int personId = 0;

            if (person != null)
            {
                personId = person.Identity;
            }

            SwarmDb.GetDatabaseForWriting().SetCommunicationTurnaroundResponded(this.OrganizationId, this.CommunicationTypeId, this.CommunicationId, dateTime, personId);

            base.Responded = true;
        }

        public void Close (DateTime dateTime, Person person)
        {
            if (!Open)
            {
                return;
            }

            int personId = 0;

            if (person != null)
            {
                personId = person.Identity;
            }

            SwarmDb.GetDatabaseForWriting().SetCommunicationTurnaroundClosed(this.OrganizationId, this.CommunicationTypeId, this.CommunicationId, dateTime, personId);

            base.Open = false;
        }
    }
}
