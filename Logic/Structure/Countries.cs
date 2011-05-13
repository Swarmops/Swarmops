using System.Collections.Generic;

using Activizr.Basic.Types;
using Activizr.Database;

namespace Activizr.Logic.Structure
{
    public class Countries : List<Country>
    {
        public static Countries GetAll ()
        {
            return FromArray(PirateDb.GetDatabase().GetAllCountries());
        }

        public static Countries GetInUse ()
        {
            return FromArray(PirateDb.GetDatabase().GetCountriesInUse());
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