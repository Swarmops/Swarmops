using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swarmops.Common.Enums
{
    // The int values are how these values get stored in the database as an int field. Therefore, do not modify the ints under any circumstance -
    // you'll change what is persisted to databases.

    public enum PositionLevel
    {
        Unknown = 0,
        System = 10,
        OrganizationDefault = 20,
        Organization = 30,
        OrganizationTop = 40,
        SuborganizationDefault = 50,
        GeographyDefault = 60,
        Geography = 70
    }
}
