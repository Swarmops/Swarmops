using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Structure;

namespace Swarmops.Logic.Swarm
{
    public class ExternalActivity: BasicExternalActivity 
    {
        private ExternalActivity (BasicExternalActivity basic): base (basic)
        {
            // empty private ctor
        }

        public static ExternalActivity FromBasic (BasicExternalActivity basic)
        {
            return new ExternalActivity(basic);
        }

        public static ExternalActivity FromIdentity (int externalActivityId)
        {
            return FromBasic(PirateDb.GetDatabaseForReading().GetExternalActivity(externalActivityId));
        }

        public static ExternalActivity Create (Organization organization, Geography geograpy, ExternalActivityType type, DateTime date, string description, Person createdByPerson)
        {
            return
                FromIdentity(PirateDb.GetDatabaseForWriting().CreateExternalActivity(organization.Identity, geograpy.Identity,
                                                                           date, type, description,
                                                                           createdByPerson.Identity));
        }

        public Geography Geography
        {
            get { return Geography.FromIdentity(this.GeographyId); }
        }


        public static int CompareDateDescending (ExternalActivity a, ExternalActivity b)
        {
            return -System.DateTime.Compare(a.DateTime, b.DateTime);
        }

        public static int CompareCreationDateDescending (ExternalActivity a, ExternalActivity b)
        {
            return -System.DateTime.Compare(a.CreatedDateTime, b.CreatedDateTime);
        }
    }
}
