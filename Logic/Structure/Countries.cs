using System.Collections.Generic;
using Swarmops.Basic.Types;
using Swarmops.Basic.Types.Structure;
using Swarmops.Database;

namespace Swarmops.Logic.Structure
{
    public class Countries : List<Country>
    {
        public static Countries GetAll()
        {
            return FromArray (SwarmDb.GetDatabaseForReading().GetAllCountries());
        }

        public static Countries GetInUse()
        {
            return FromArray (SwarmDb.GetDatabaseForReading().GetCountriesInUse());
        }

        public static Countries FromArray (BasicCountry[] basicArray)
        {
            Countries result = new Countries {Capacity = (basicArray.Length*11/10)};

            foreach (BasicCountry basic in basicArray)
            {
                result.Add (Country.FromBasic (basic));
            }

            return result;
        }
    }
}