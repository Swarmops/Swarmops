using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swarmops.Basic.Types.Structure;
using Swarmops.Database;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Structure
{
    public class GeographyUpdates:PluralBase<GeographyUpdates,GeographyUpdate,BasicGeographyUpdate>
    {
        static public GeographyUpdates GetUnprocessed()
        {
            return FromArray (SwarmDb.GetDatabaseForReading().GetUnprocessedGeographyUpdates());
        }

        static public GeographyUpdates GetCreatedSince (DateTime dateTimeSince)
        {
            return FromArray (SwarmDb.GetDatabaseForReading().GetGeographyUpdatesSince (dateTimeSince));
        }
    }
}
