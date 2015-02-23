using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Common.Attributes;

namespace Swarmops.Common.Enums
{
    // The int values are how these values get stored in the database as an int field. Therefore, do not modify the ints under any circumstance -
    // you'll change what is persisted to databases.

    [DbEnumField("PositionLevel")]
    public enum PositionLevel
    {
        Unknown = 0,
        Systemwide = 10,
        OrganizationwideDefault = 20,
        Organizationwide = 30,
        OrganizationTop = 40,
        GeographyDefault = 50,
        SuborganizationwideDefault = 60,
        SuborganizationTopDefault = 70,
        SuborganizationGeographyDefault = 80,
        Geography = 90
    }
}


