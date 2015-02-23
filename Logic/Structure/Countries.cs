using System.Collections.Generic;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Structure;
using Swarmops.Database;
using Swarmops.Logic.Cache;
using Swarmops.Logic.Support;

namespace Swarmops.Logic.Structure
{
    public class Countries : PluralBase<Countries, Country, BasicCountry>
    {
        public static Countries All
        {
            get
            {
                // Cached - the list of countries doesn't change that much

                Countries result = (Countries) GuidCache.Get ("Countries.All");
                if (result == null)
                {
                    result = Countries.GetAll();
                    GuidCache.Set ("Countries.All", result);
                }

                return result;
            }
        }

        private static Countries GetAll() // private - uncached version shouldn't be used outside this class
        {
            return FromArray (SwarmDb.GetDatabaseForReading().GetAllCountries());
        }

        public static Countries GetInUse()
        {
            return FromArray (SwarmDb.GetDatabaseForReading().GetCountriesInUse());
        }

    }
}