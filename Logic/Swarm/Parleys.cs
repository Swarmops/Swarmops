using System.Linq;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Swarm
{
    public class Parleys : PluralBase<Parleys, Parley, BasicParley>
    {
        public Parleys WhereUnattested
        {
            get
            {
                Parleys result = new Parleys();
                result.AddRange (this.Where (parley => !parley.Attested));

                return result;
            }
        }

        public static Parleys ForOrganization (Organization organization)
        {
            return ForOrganization (organization, false);
        }

        public static Parleys ForOrganization (Organization organization, bool includeClosed)
        {
            if (includeClosed)
            {
                return FromArray (SwarmDb.GetDatabaseForReading().GetParleys (organization));
            }
            return FromArray (SwarmDb.GetDatabaseForReading().GetParleys (organization, DatabaseCondition.OpenTrue));
        }

        public static Parleys ForOwner (Person person)
        {
            return ForOwner (person, false);
        }

        public static Parleys ForOwner (Person person, bool includeClosed)
        {
            if (includeClosed)
            {
                return FromArray (SwarmDb.GetDatabaseForReading().GetParleys (person));
            }
            return FromArray (SwarmDb.GetDatabaseForReading().GetParleys (person, DatabaseCondition.OpenTrue));
        }
    }
}