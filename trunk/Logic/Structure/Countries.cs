using System.Collections.Generic;
using Swarmops.Basic.Types;
using Swarmops.Database;

namespace Swarmops.Logic.Structure
{
    public class Countries : List<Country>
    {
        public static Countries GetAll ()
        {
            return FromArray(PirateDb.GetDatabaseForReading().GetAllCountries());
        }

        public static Countries GetInUse ()
        {
            return FromArray(PirateDb.GetDatabaseForReading().GetCountriesInUse());
        }

        public static Countries FromArray (BasicCountry[] basicArray)
        {
            var result = new Countries {Capacity = (basicArray.Length*11/10)};

            foreach (BasicCountry basic in basicArray)
            {
                result.Add(Country.FromBasic(basic));
            }

            return result;
        }
    }
}