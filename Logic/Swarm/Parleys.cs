using System;
using System.Collections.Generic;
using System.Text;
using Swarmops.Basic.Types;
using Swarmops.Database;
using Swarmops.Logic.Structure;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Swarm
{
    public class Parleys: PluralBase<Parleys,Parley,BasicParley>
    {
        public static Parleys ForOrganization (Organization organization)
        {
            return ForOrganization(organization, false);
        }

        public static Parleys ForOrganization (Organization organization, bool includeClosed)
        {
            if (includeClosed)
            {
                return FromArray(PirateDb.GetDatabaseForReading().GetParleys(organization));
            }
            else
            {
                return FromArray(PirateDb.GetDatabaseForReading().GetParleys(organization, DatabaseCondition.OpenTrue));
            }
        }

        public static Parleys ForOwner (Person person)
        {
            return ForOwner(person, false);
        }

        public static Parleys ForOwner (Person person, bool includeClosed)
        {
            if (includeClosed)
            {
                return FromArray(PirateDb.GetDatabaseForReading().GetParleys(person));
            }
            else
            {
                return FromArray(PirateDb.GetDatabaseForReading().GetParleys(person, DatabaseCondition.OpenTrue));
            }
        }
    }
}
