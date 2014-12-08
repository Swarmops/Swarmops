using System;
using Swarmops.Basic.Enums;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Structure;

namespace Swarmops.Logic.Swarm
{
    public class ExternalActivity : BasicExternalActivity
    {
        private ExternalActivity (BasicExternalActivity basic) : base (basic)
        {
            // empty private ctor
        }

        public Geography Geography
        {
            get { return Geography.FromIdentity (GeographyId); }
        }

        public static ExternalActivity FromBasic (BasicExternalActivity basic)
        {
            return new ExternalActivity (basic);
        }

        public static ExternalActivity FromIdentity (int externalActivityId)
        {
            return FromBasic (SwarmDb.GetDatabaseForReading().GetExternalActivity (externalActivityId));
        }

        public static ExternalActivity Create (Organization organization, Geography geograpy, ExternalActivityType type,
            DateTime date, string description, Person createdByPerson)
        {
            return
                FromIdentity (SwarmDb.GetDatabaseForWriting()
                    .CreateExternalActivity (organization.Identity, geograpy.Identity,
                        date, type, description,
                        createdByPerson.Identity));
        }


        public static int CompareDateDescending (ExternalActivity a, ExternalActivity b)
        {
            return -DateTime.Compare (a.DateTime, b.DateTime);
        }

        public static int CompareCreationDateDescending (ExternalActivity a, ExternalActivity b)
        {
            return -DateTime.Compare (a.CreatedDateTime, b.CreatedDateTime);
        }
    }
}