using System;
using System.Collections.Generic;
using System.Text;
using Activizr.Logic.Structure;
using Activizr.Logic.Support;
using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Pirates
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
                return FromArray(PirateDb.GetDatabase().GetParleys(organization));
            }
            else
            {
                return FromArray(PirateDb.GetDatabase().GetParleys(organization, DatabaseCondition.OpenTrue));
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
                return FromArray(PirateDb.GetDatabase().GetParleys(person));
            }
            else
            {
                return FromArray(PirateDb.GetDatabase().GetParleys(person, DatabaseCondition.OpenTrue));
            }
        }
    }
}
